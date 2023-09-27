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
    public class CadenaPrecioService : ICadenaPrecioService
    {    
        private readonly ICadenaPrecioRepository cadenaRepository;

        public CadenaPrecioService(ICadenaPrecioRepository cadenaRepository)
        {
            this.cadenaRepository = cadenaRepository;
        }
        public async Task<List<CadenaPrecio>> ListarCadena()
        {
            return await cadenaRepository.ListarCadena();
        }
    }
}
