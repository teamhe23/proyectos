using Domain.Models;
using Domain.Repositories;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class TipoOrdenCompraService :ITipoOrdenCompraService
    {
        private readonly ITipoOrdenCompraRepository tipoOrdenCompraRepository;

        public TipoOrdenCompraService(ITipoOrdenCompraRepository tipoOrdenCompraRepository)
        {
            this.tipoOrdenCompraRepository = tipoOrdenCompraRepository;
        }
        public async Task<List<TipoOrdenCompra>>ListarTipoOrdenCompra(string pOrigen)
        {
            return await tipoOrdenCompraRepository.ListarTipoOrdenCompra(pOrigen);
        }
    }
}
