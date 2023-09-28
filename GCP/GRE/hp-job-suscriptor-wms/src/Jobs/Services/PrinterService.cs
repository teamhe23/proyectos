using System;
using Domain.Models.Properties;
using Domain.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jobs.Services
{
    public class PrinterService : IPrinterService
    {
        private readonly ILogger<PrinterService> _logger;
        private readonly OracleProperties _oracle;
        private readonly Settigns _settings;

        public PrinterService(ILogger<PrinterService> logger,
                              IOptions<OracleProperties> oracle,
                              IOptions<Settigns> settings)
        {
            _logger = logger;
            _oracle = oracle.Value;
            _settings = settings.Value;
        }

        public void Print(string message)
        {
            _logger.LogInformation(message);
        }

        public void PrintInfoJson()
        {
            _logger.LogInformation($@"***Información Oracle{Environment.NewLine}" +
                                   $"data_source:{_oracle.DataSource}{Environment.NewLine}" +
                                   $"user_id:{_oracle.User}{Environment.NewLine}" +
                                   $"***Información Settigns{Environment.NewLine}" +
                                   $"TiempoEsperaSegundos: {_settings.TiempoEsperaSegundos}{Environment.NewLine}" +
                                   $"RutaGCPCredenciales: {_settings.RutaGCPCredenciales}");
        }

        public void PrintInicioProceso()
        {
            _logger.LogInformation($"========================={Environment.NewLine}"
                                 + $"  INICIO DE PROCESO WMS{Environment.NewLine}"
                                 + $"========================={Environment.NewLine}");
        }
    }
}
