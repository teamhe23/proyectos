using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories.Oracle
{
    public interface IOrderRepository
    {
        Task<List<Transferencia>> GetTransferencia();

        Task<Model> GetModel(int P_ID_RET, string P_IND_ORD);

        Task<bool> Confirma_Envio(int P_ID_RET, string P_IND_ORD, String nroBIR);
    }
}
