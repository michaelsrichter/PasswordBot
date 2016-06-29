using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace dx.misv.passwordbot.core
{
    public class RegisterUnityContainer
    {
        public static UnityContainer Get()
        {
            var container = new UnityContainer();

            container.RegisterType<IPasswordCore, PasswordCore>();
            container.RegisterType<IPasswordProvider, RandomPasswordProvider>();
            container.RegisterType<IStringSource, WordStringSource>();

            return container;
        } 
    }
}
