using Castle.Windsor;
using Castle.Windsor.Installer;
using System.Reflection;

namespace BeanTraderClient.DependencyInjection
{
    public static class Bootstrapper
    {
        private static IWindsorContainer container;
        private static readonly object syncRoot = new object();

        public static IWindsorContainer Container
        {
            get
            {
                if (container == null)
                {
                    lock (syncRoot)
                    {
                        if (container == null)
                        {
                            using (var installer = new WindsorContainer())
                            {
                                container = installer.Install(
                                    FromAssembly.Instance(Assembly.GetCallingAssembly()));
                            }
                        }
                    }
                }

                return container;
            }
        }
    }
}
