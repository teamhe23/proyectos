using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Domain.Services;
using Jobs.Services;
using Domain.Models.Properties;
using Domain.Repositories;
using Data.Oracle.Repositories;

namespace IoC
{
    public static class IoCBootstrapper
    {
        public static IServiceCollection Bootstrap()
        {
            var services = new ServiceCollection();

            var config = new ConfigurationBuilder()
                         .SetBasePath(Directory.GetCurrentDirectory())
                         .AddJsonFile("appsettings.json", true, true)
                         .AddEnvironmentVariables()
                         .Build();

            services.AddLogging(log => log.AddConsole());

            services.Configure<OracleProperties>(info => config.GetSection("Oracle").Bind(info));
            services.Configure<Settigns>(info => config.GetSection("Settigns").Bind(info));
            services.Configure<PubSubProperties>(info => config.GetSection("PubSub").Bind(info));

            services.AddSingleton<IPrinterService, PrinterService>();
            services.AddSingleton<IPubSubService, PubSubService>();

            services.AddSingleton<IModeloWMSRepository, ModeloWMSRepository>();

            return services;
        }
    }
}
