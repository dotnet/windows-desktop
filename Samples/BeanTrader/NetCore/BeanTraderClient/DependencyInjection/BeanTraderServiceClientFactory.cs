using System;
using System.Configuration;
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

        public BeanTraderServiceClient GetServiceClient()
        {
            var binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            var endpointAddress = new EndpointAddress(new Uri(ConfigurationManager.AppSettings["BeanTraderEndpointAddress"]), new DnsEndpointIdentity("BeanTrader"));

            return new BeanTraderServiceClient(new InstanceContext(CallbackHandler), binding, endpointAddress);
        }
    }
}