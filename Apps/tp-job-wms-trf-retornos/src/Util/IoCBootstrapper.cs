using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Domain.Models.Properties;
using Domain.Services;
using Jobs.Services;
using Data.Oracle.Repositories;
using Domain.Repositories.Oracle;
using Domain.Repositories.WMS;
using Data.WMS.Repositories;
using Domain.Models;

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

            services.Configure<OracleProperties>(info => config.GetSection("db:oracle").Bind(info));
            services.Configure<Settigns>(info => config.GetSection("Settigns").Bind(info));
            services.Configure<ApiWMSProperties>(info => config.GetSection("api:wms").Bind(info));

            services.AddSingleton<IPrinterService, PrinterService>();
            services.AddTransient<ISettignsService, SettignsService>();
            services.AddTransient<IWMSService, WMSService>();

            services.AddSingleton<ITipoIntegracionRepository, TipoIntegracionRepository>();
            services.AddSingleton<ILogsRepository, LogsRepository>();
            services.AddSingleton<IOrderRepository, OrderRepository>();

            services.AddSingleton<ISucursalWMSRepository, SucursalWMSRepository>();
            return services;
        }
    }
}
