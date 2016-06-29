param(
	[Parameter(Mandatory=$True)]
	[string]$buildNumber,
	[Parameter(Mandatory=$True)]
	[string]$CertificateThumbprint,
	[Parameter(Mandatory=$True)]
	[string]$ApplicationId,
	[Parameter(Mandatory=$True)]
	[string]$TenantId,
	[Parameter(Mandatory=$True)]
	[string]$sourceFile,
	[Parameter(Mandatory=$True)]
	[string]$templateFilePath,
	[Parameter(Mandatory=$True)]
	[string]$subscriptionId,
	[Parameter(Mandatory=$True)]
	[string]$resourceGroupName,
	[Parameter(Mandatory=$True)]
	[string]$resourceGroupLocation,
	[Parameter(Mandatory=$True)]
	[string]$storageAccountKey,
	[Parameter(Mandatory=$True)]
	[string]$storageAccountName,
	[Parameter(Mandatory=$True)]
	[string]$artifactContainerName,
	[Parameter(Mandatory=$True)]
	[string]$configurationScriptFile
	)

# Use ServicePrincipal to Authenticate to Azure https://azure.microsoft.com/en-us/documentation/articles/resource-group-authenticate-service-principal/#create-ad-application-with-certificate
Add-AzureRmAccount -ServicePrincipal `
-CertificateThumbprint $CertificateThumbprint `
-ApplicationId $ApplicationId `
-TenantId $TenantId


# A name for this particular deployment
$deploymentName = $resourceGroupName +"-deploy-" + $buildNumber;

# The Azure Storage Context. We need this for working with files in Azure Storage.
$context = New-AzureStorageContext -StorageAccountName $storageAccountName `
-StorageAccountKey $storageAccountKey `
-Protocol Https

# The customScriptExtension  script is what will run on the VM. 
# This needs to be in Azure Storage so our template can use it.
$configurationScriptName =  "configScript" + $resourceGroupName +  $buildNumber + ".ps1"

$artifactContainer = Get-AzureStorageContainer -Context $context | Where-Object {$_.Name -eq $artifactContainerName }

if (!$artifactContainer){
   New-AzureStorageContainer -Name $artifactContainerName -Context $context
}

Set-AzureStorageBlobContent -File $configurationScriptFile -Container $artifactContainerName -Blob $configurationScriptName -BlobType Block -Context $context -Force


# A temporary access token so that our template can retrieve the custom Script 
# from blob storage and run it on the VM
$scriptToken = New-AzureStorageBlobSASToken `
-Container $artifactContainerName -Blob $configurationScriptName -Permission r -ExpiryTime (Get-Date).AddHours(2.0) -Context $context
$downloadScript = Get-AzureStorageBlob -Blob $configurationScriptName -Container $artifactContainerName -Context $context

# The script URL and token that will get passed to the template
$scriptUrl = $downloadScript.ICloudBlob.Uri.AbsoluteUri + $scriptToken

# The name of your MSI file when it's put into Azure
$uploadBlob = "app" + $resourceGroupName +  $buildNumber + ".msi"


# Upload the MSI file to Azure
Set-AzureStorageBlobContent -File $sourceFile -Container $artifactContainerName -Blob $uploadBlob -BlobType Block -Context $context -Force
# A temporary access token so that our template can retrieve the MSI
# from blob storage and install it on the VM
$token = New-AzureStorageBlobSASToken -Container $artifactContainerName -Blob $uploadBlob -Permission r -ExpiryTime (Get-Date).AddHours(2.0) -Context $context
$downloadBlob = Get-AzureStorageBlob -Blob $uploadBlob -Container $artifactContainerName -Context $context

# The MSI URL and token that will get passed to the template
$blobUrl = $downloadBlob.ICloudBlob.Uri.AbsoluteUri + $token

# A parameters object that contain all the parameters we will pass into our template

$fixName = $resourceGroupName.ToLower();
$parameters = @{
"adminUser"= $fixName;
"adminPassword"= "A" + $fixName + $buildNumber;
"availabilitySet"= $fixName + "as";
"vmName"= "vm-" + $fixName + $buildNumber;
"nicName"="nic-" + $fixName + $buildNumber;
"nsgName"= "nsg-" + $fixName
"publicIP"="ip-" + $fixName + $buildNumber;
"vnetName"="vnet-" + $fixName;
"storageAccount"="store" + $fixName;
"customScriptExtensionUri" = $scriptUrl;
"installFileUri" = $blobUrl
"customScriptExtensionCommand" = "powershell.exe -File " + $configurationScriptName + " -installFile " + $uploadBlob;
"location" = $resourceGroupLocation
};
$ErrorActionPreference = "Stop"

# select subscription
Write-Host "Selecting subscription '$subscriptionId'";
Select-AzureRmSubscription -SubscriptionID $subscriptionId;

#Create or check for existing resource group
$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction SilentlyContinue
if(!$resourceGroup)
{
    Write-Host "Resource group '$resourceGroupName' does not exist. To create a new resource group, please enter a location.";
    if(!$resourceGroupLocation) {
        $resourceGroupLocation = Read-Host "resourceGroupLocation";
    }
    Write-Host "Creating resource group '$resourceGroupName' in location '$resourceGroupLocation'";
    New-AzureRmResourceGroup -Name $resourceGroupName -Location $resourceGroupLocation
}
else{
    Write-Host "Using existing resource group '$resourceGroupName'";
}

# Start the deployment
Write-Host "Starting deployment...";

New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFilePath -TemplateParameterObject $parameters -Name $deploymentName

# Restart the VM to let all dependencies finish installing
Restart-AzureRmVM -Name $parameters.vmName -ResourceGroupName $resourceGroupName