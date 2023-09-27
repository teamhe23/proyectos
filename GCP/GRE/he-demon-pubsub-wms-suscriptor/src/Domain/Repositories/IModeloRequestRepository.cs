
using Domain.Models;

namespace Domain.Repositories
{
    public interface IModeloRequestRepository
    {
        Task<long> InsModeloRequest(ModeloRequest modeloRequest);
    }
}
