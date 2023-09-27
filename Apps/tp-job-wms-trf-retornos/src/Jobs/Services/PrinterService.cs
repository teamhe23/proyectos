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

        public PrinterService(ILogger<PrinterService> logger, IOptions<OracleProperties> oracle)
        {
            _logger = logger;
            _oracle = oracle.Value;
        }

        public void Print(string message)
        {
            _logger.LogInformation(message);
        }

        public void PrintInfoJson()
        {
            _logger.LogInformation($"***Información Oracle{Environment.NewLine}" +
                $"data_source:{_oracle.DataSource}{Environment.NewLine}" +
                $"user_id:{_oracle.User}");
        }

        public void PrintInicioWMS()
        {
            _logger.LogInformation($"========================={Environment.NewLine}"
                                 + $"  INICIO DE PROCESO WMS{Environment.NewLine}"
                                 + $"========================={Environment.NewLine}");
        }
        public void PrintFinWMS()
        {
            _logger.LogInformation($"========================={Environment.NewLine}"
                                 + $"  Fin DE PROCESO WMS{Environment.NewLine}"
                                 + $"========================={Environment.NewLine}");
        }
    }
}
