  param(
	[string]
	$installFile
	)
  
  function Disable-InternetExplorerESC {
    $AdminKey = "HKLM:\SOFTWARE\Microsoft\Active Setup\Installed Components\{A509B1A7-37EF-4b3f-8CFC-4F3A74704073}"
    $UserKey = "HKLM:\SOFTWARE\Microsoft\Active Setup\Installed Components\{A509B1A8-37EF-4b3f-8CFC-4F3A74704073}"
    Set-ItemProperty -Path $AdminKey -Name "IsInstalled" -Value 0
    Set-ItemProperty -Path $UserKey -Name "IsInstalled" -Value 0
    Write-Host "IE Enhanced Security Configuration (ESC) has been disabled." -ForegroundColor Green
}

Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Force
Install-WindowsFeature -Name Web-Server -IncludeAllSubFeature -IncludeManagementTools

Write-Output "downloadUrl = " + $downloadUrl

Write-Output "Web Server Installed"

Disable-InternetExplorerESC

Write-Output "IE ESC Disabled"

Import-Module WebAdministration

Set-ItemProperty -Path IIS:\AppPools\DefaultAppPool -Name enable32BitAppOnWin64 -Value $true

Write-Output "App Pool Updated to Allow 32 Bit apps"

Import-Module NetSecurity
New-NetFirewallRule -Name BotApi -Enabled True -Action Allow -Protocol tcp -Direction Inbound -DisplayName BotApi -LocalPort 81 

Write-Output "Firewall Updated"

Start-Process -FilePath ($PSScriptRoot + "\" + $installFile) -ArgumentList '/qn' -Wait -Verbose

Write-Output "Installed MSI!"

Invoke-WebRequest -Uri https://download.microsoft.com/download/E/4/1/E4173890-A24A-4936-9FC9-AF930FE3FA40/NDP461-KB3102436-x86-x64-AllOS-ENU.exe -OutFile c:\net.exe

Start-Process -FilePath 'c:\net.exe'  -ArgumentList '/q /norestart' -Wait -Verb RunAs

Write-Output ".MNET 4.6.1 Installed"
