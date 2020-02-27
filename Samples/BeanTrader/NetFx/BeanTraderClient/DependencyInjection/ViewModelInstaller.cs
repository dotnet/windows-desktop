using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System;
using System.Reflection;

namespace BeanTraderClient.DependencyInjection
{
    public class ViewModelInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container?.Register(Classes.FromAssembly(Assembly.GetCallingAssembly())
                .Where(t => t.Name.EndsWith("ViewModel", StringComparison.Ordinal))
                .LifestyleTransient());
        }
    }
}
