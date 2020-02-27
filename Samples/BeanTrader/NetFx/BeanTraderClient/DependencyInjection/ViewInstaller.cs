using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace BeanTraderClient.DependencyInjection
{
    public class ViewInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container?.Register(Classes.FromAssembly(Assembly.GetCallingAssembly())
                .BasedOn<Window>()
                .OrBasedOn(typeof(Page))
                .LifestyleTransient());
        }
    }
}
