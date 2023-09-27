using Domain.Helpers;
using Domain.Models;
using Domain.Models.Properties;
using Domain.Repositories;
using Domain.Services;
using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Options;

namespace Services.Services
{
    public class PubSubService : IPubSubService
    {
        private readonly PubSubProperty _pubSubProperty;
        private readonly Settings _settings;
        private readonly IModeloRequestRepository _modeloRequestRepository;

        public PubSubService(IOptions<PubSubProperty>   pubSubProperty,
                             IOptions<Settings>         settings,
                             IModeloRequestRepository modeloRequestRepository)
        {
            _pubSubProperty             = pubSubProperty.Value;
            _settings                   = settings.Value;
            _modeloRequestRepository    = modeloRequestRepository;
        }

        public async Task ExtraerTrama()
        {
            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(_pubSubProperty.Proyecto, _pubSubProperty.Suscripcion);
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", _settings.RutaGCPCredenciales);
            SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);

            await subscriber.StartAsync((PubsubMessage pubsubMessage, CancellationToken cancel) =>
            {

                string text = System.Text.Encoding.UTF8.GetString(pubsubMessage.Data.ToArray());
                Console.WriteLine($"MensajeId: {pubsubMessage.MessageId}. Data: {text.Substring(0, text.Length <= 50 ? text.Length : 50)}");

                if (!pubsubMessage.Attributes.Any())
                {
                    Console.WriteLine($"Mensaje sin atributo");
                }

                var atributo = pubsubMessage.Attributes.Where(atr => atr.Key == "tipo").Select(atr => atr.Value).FirstOrDefault();
                var idTipo = 0;

                switch (atributo)
                {
                    case "outbound-load":
                        idTipo = TipoModeloRequest.OutboundLoadsExport;
                        break;
                    case "verificacion_asn":
                        idTipo = TipoModeloRequest.InboundShipmentVerificationsExport;
                        break;
                    case "inventory_history":
                        idTipo = TipoModeloRequest.InventoryHistoryExport;
                        break;
                    case "order_crossdock":
                        idTipo = TipoModeloRequest.OrderVerificationExport;
                        break;
                }

                Int64 idModelo = _modeloRequestRepository.InsModeloRequest(new ModeloRequest { Modelo = text, IdTipo = idTipo, MessageId = pubsubMessage.MessageId }).Result;
                Console.WriteLine($"idModelo generado: {idModelo}");

                return Task.FromResult(SubscriberClient.Reply.Ack);
            });
        }
    }
}
