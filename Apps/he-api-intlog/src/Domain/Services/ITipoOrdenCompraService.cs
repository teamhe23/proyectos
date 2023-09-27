using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface ITipoOrdenCompraService
    {
        Task<List<TipoOrdenCompra>>ListarTipoOrdenCompra(string porigen);
    }
}
