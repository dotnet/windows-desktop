using System.ServiceModel;

namespace BeanTraderClient.DependencyInjection
{
    public class BeanTraderServiceClientFactory
    {
        private BeanTraderServiceCallback CallbackHandler { get; }

        public BeanTraderServiceClientFactory(BeanTraderServiceCallback callbackHandler)
        {
            CallbackHandler = callbackHandler;
        }

        public BeanTraderServiceClient GetServiceClient() => new BeanTraderServiceClient(new InstanceContext(CallbackHandler));
    }
}