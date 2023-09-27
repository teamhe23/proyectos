using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Domain.Services;
using Domain.Models.Properties;
using Domain.Repositories;
using Data.Oracle.Repositories;
using Services.Services;

namespace Facade.Helpers
{
    internal static class IoCBootstrapper
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

            services.Configure<OracleProperty>(info => config.GetSection("OracleDatabase").Bind(info));
            services.Configure<Settings>(info => config.GetSection("Settings").Bind(info));
            services.Configure<PubSubProperty>(info => config.GetSection("PubSub").Bind(info));

            services.AddSingleton<IPrinterService, PrinterService>();
            services.AddSingleton<IPubSubService, PubSubService>();

            services.AddSingleton<IModeloRequestRepository, ModeloRequestRepository>();

            return services;
        }
    }
}
