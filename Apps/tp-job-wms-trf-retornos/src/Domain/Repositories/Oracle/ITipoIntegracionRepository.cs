using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories.Oracle
{
    public interface ITipoIntegracionRepository
    {
        Task<List<TipoIntegracion>> get(Int64? IdProceso);
    }
}
