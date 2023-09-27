using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IWMSService
    {
        Task<List<TipoIntegracion>> getIntegracion();
        Task<List<Transferencia>> GetTransferencia();
        Task<Model> GetModel(int P_ID_RET, string P_IND_ORD);
        Task PostAllOrder(Model Model, int P_ID_RET, string P_IND_ORD);
    }
}