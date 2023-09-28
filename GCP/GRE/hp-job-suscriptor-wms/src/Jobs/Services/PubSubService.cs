using Domain.Models;
using Domain.Models.Properties;
using Domain.Repositories;
using Domain.Services;
using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobs.Services
{
    public class PubSubService : IPubSubService
    {
        private readonly PubSubProperties _pubSubProperties;
        private readonly Settigns _settigns;
        private readonly IModeloWMSRepository _modeloWMSRepository;

        public PubSubService(IOptions<PubSubProperties> pubSubProperties,
                             IOptions<Settigns> settigns,
                             IModeloWMSRepository modeloWMSRepository)
        {
            _pubSubProperties       = pubSubProperties.Value;
            _settigns               = settigns.Value;
            _modeloWMSRepository    = modeloWMSRepository;
        }

        public async Task ExtraerTrama()
        {
            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(_pubSubProperties.Proyecto, _pubSubProperties.Suscripcion);
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", _settigns.RutaGCPCredenciales);
            SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);

            await subscriber.StartAsync((PubsubMessage pubsubMessage, CancellationToken cancel) =>
            {
                string text = System.Text.Encoding.UTF8.GetString(pubsubMessage.Data.ToArray());
                Console.WriteLine($"MensajeId: {pubsubMessage.MessageId}. Data: {text.Substring(0, text.Length <= 50 ? text.Length : 50)}");

                Int64 idModelo = _modeloWMSRepository.InsModeloWMS(new ModeloWNS { Cadena = text }).Result;
                Console.WriteLine($"idModelo generado: {idModelo}");

                return Task.FromResult(SubscriberClient.Reply.Ack);
            });
        }
    }
}
