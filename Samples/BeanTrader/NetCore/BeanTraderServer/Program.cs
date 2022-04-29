using CoreWCF.Configuration;
using CoreWCF.Security;
using Microsoft.AspNetCore.Builder;
using Serilog;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace BeanTraderServer
{
    class Program
    {
        async static Task Main()
        {
            ConfigureLogging();

            var builder = WebApplication.CreateBuilder();

            // Set NetTcp port (previously this was done in configuration,
            // but CoreWCF requires it be done in code)
            builder.WebHost.UseNetTcp(8090);

            // Add CoreWCF services to the ASP.NET Core app's DI container
            builder.Services.AddServiceModelServices();
            builder.Services.AddServiceModelConfigurationManagerFile("wcf.config");

            var app = builder.Build();

            // Configure CoreWCF endpoints in the ASP.NET Core host
            app.UseServiceModel(serviceBuilder =>
            {
                serviceBuilder.ConfigureServiceHostBase<BeanTrader>(beanTraderServiceHost =>
                {
                    // This code is copied from the old ServiceHost setup and configures
                    // the local cert used for authentication.
                    // For demo purposes, this just loads the certificate from disk so that no one needs to install an
                    // untrustworthy self-signed cert or load from KeyVault (which would complicate the sample)
                    var certPath = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "BeanTrader.pfx");
                    beanTraderServiceHost.Credentials.ServiceCertificate.Certificate = new X509Certificate2(certPath, "password");
                    beanTraderServiceHost.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
                });
            });

            await app.StartAsync();

            Log.Information("Bean Trader Service listening");
            WaitForExitSignal();
            Log.Information("Shutting down...");

            await app.StopAsync();
        }

        private static void WaitForExitSignal()
        {
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }

        private static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("Logging initialized");
        }
    }
}
