using Domain.Models.Properties;
using Domain.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Services.Services
{
    public class PrinterService : IPrinterService
    {
        private readonly ILogger<PrinterService> _logger;
        private readonly OracleProperty _oracle;
        private readonly Settings _settings;
        private readonly PubSubProperty _pubsub;

        public PrinterService(ILogger<PrinterService>   logger,
                              IOptions<OracleProperty>  oracle,
                              IOptions<Settings>        settings,
                              IOptions<PubSubProperty>  pubsub)
        {
            _logger         = logger;
            _oracle         = oracle.Value;
            _settings       = settings.Value;
            _pubsub         = pubsub.Value;
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
                                   $"RutaGCPCredenciales: {_settings.RutaGCPCredenciales}{Environment.NewLine}" +
                                   $"***Información PubSub{Environment.NewLine}" +
                                   $"Proyecto: {_pubsub.Proyecto}{Environment.NewLine}" +
                                   $"Suscripcion: {_pubsub.Suscripcion}{Environment.NewLine}");
        }
    }
}
