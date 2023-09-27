using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface ITipoOrdenCompraRepository
    {
        Task<List<TipoOrdenCompra>>ListarTipoOrdenCompra(string porigen);
    }
}
