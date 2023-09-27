
using Domain.Helpers;
using Domain.Models;
using Domain.Models.Properties;
using Domain.Repositories.Oracle;
using Domain.Repositories.WMS;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Jobs.Services
{
    public class WMSService : IWMSService
    {
        private readonly ITipoIntegracionRepository _TipoIntegracionRepository;
        private readonly ILogsRepository _LogsRepository;
        private readonly IOrderRepository _orderRepository;

        private readonly ISucursalWMSRepository _sucursalWMSRepository;
        public WMSService(ITipoIntegracionRepository TipoIntegracionRepository, ILogsRepository logsRepository, IOrderRepository orderRepository, ISucursalWMSRepository sucursalWMSRepository)
        {
            _TipoIntegracionRepository = TipoIntegracionRepository;
            _LogsRepository = logsRepository;
            _orderRepository = orderRepository;

            _sucursalWMSRepository = sucursalWMSRepository;
        }
        public async Task<List<TipoIntegracion>> getIntegracion()
        {
            return await _TipoIntegracionRepository.get(TipoInterfaces.WMS_STORE);
        }

        public async Task<List<Transferencia>> GetTransferencia()
        {
            return await _orderRepository.GetTransferencia();
        }

        public async Task<Model> GetModel(int P_ID_RET, string P_IND_ORD)
        {
            return await _orderRepository.GetModel(P_ID_RET, P_IND_ORD);
        }

        public async Task PostAllOrder(Model Model, int P_ID_RET, string P_IND_ORD)
        {
            try
            {
                await _LogsRepository.Post(new Logs()
                {
                    ID_TIPO = TipoInterfaces.WMS_STORE,
                    TRAMA = null,
                    MENSAJE = "Inicia envio ordenes de WMS",
                    IDENTIFICADOR = P_ID_RET
                });

                await _LogsRepository.Post(new Logs()
                {
                    ID_TIPO = TipoInterfaces.WMS_STORE,
                    TRAMA = Model.data,
                    MENSAJE = "Request de envio ordenes a WMS",
                    IDENTIFICADOR = P_ID_RET
                });

                await _LogsRepository.Post(new Logs()
                {
                    ID_TIPO = TipoInterfaces.WMS_STORE,
                    TRAMA = null,
                    MENSAJE = "Enviando interface ordenes a WMS",
                    IDENTIFICADOR = P_ID_RET
                });

                HttpResponseMessage status = await _sucursalWMSRepository.Post(Model.data);

                if (status.IsSuccessStatusCode)
                {
                    var EstadoEnvio = await _orderRepository.Confirma_Envio(P_ID_RET, P_IND_ORD, Model.nroBIR);//se esta agregando el nro de bir para actualizar en la tabla

                    await _LogsRepository.Post(new Logs()
                    {
                        ID_TIPO = TipoInterfaces.WMS_STORE,
                        TRAMA = status.Content.ReadAsStringAsync().Result,
                        MENSAJE = "Response de envio ordenes a WMS (OK)",
                        IDENTIFICADOR = P_ID_RET
                    });
                }
                else
                {
                    await _LogsRepository.Post(new Logs()
                    {
                        ID_TIPO = TipoInterfaces.WMS_STORE,
                        TRAMA = status.Content.ReadAsStringAsync().Result,
                        MENSAJE = "Response de envio ordenes a WMS (ERROR)",
                        IDENTIFICADOR = P_ID_RET
                    });
                }
            }
            catch (Exception ex)
            {
                await _LogsRepository.Post(new Logs()
                {
                    ID_TIPO = TipoInterfaces.WMS_STORE,
                    TRAMA = null,
                    MENSAJE = "ERROR:" + ex.Message,
                    IDENTIFICADOR = P_ID_RET
                });
            }

            await _LogsRepository.Post(new Logs()
            {
                ID_TIPO = TipoInterfaces.WMS_STORE,
                TRAMA = null,
                MENSAJE = "Fin envio ordenes de WMS",
                IDENTIFICADOR = P_ID_RET
            });

            return;
        }
    }
}
