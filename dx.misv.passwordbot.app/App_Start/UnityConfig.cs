using Microsoft.Practices.Unity;
using System.Web.Http;
using dx.misv.passwordbot.app.Controllers;
using dx.misv.passwordbot.app.Dialogs;
using dx.misv.passwordbot.app.Services;
using dx.misv.passwordbot.core;
using Unity.WebApi;

namespace dx.misv.passwordbot.app
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = RegisterUnityContainer.Get();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<IWordService, ResourceWordService>(new ContainerControlledLifetimeManager());
            container.RegisterType(typeof(PasswordDialog));
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}