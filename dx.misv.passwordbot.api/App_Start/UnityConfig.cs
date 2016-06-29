using Microsoft.Practices.Unity;
using System.Web.Http;
using dx.misv.passwordbot.core;
using Unity.WebApi;

namespace dx.misv.passwordbot.api
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(RegisterUnityContainer.Get());
        }
    }
}