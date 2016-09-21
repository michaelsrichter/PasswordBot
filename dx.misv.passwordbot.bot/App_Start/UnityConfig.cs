using Microsoft.Practices.Unity;
using System.Web.Http;
using dx.misv.passwordbot.core.Dialogs;
using dx.misv.passwordbot.core.Services;
using Unity.WebApi;

namespace dx.misv.passwordbot.bot
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();
            container.RegisterType<IWordService, ResourceWordService>(new ContainerControlledLifetimeManager());
            container.RegisterType(typeof(PasswordDialog));
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}