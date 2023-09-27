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
    public class SucursalesService :ISucursalesService
    {

        private readonly ISucursalesRepository sucursalesRepository;

        public SucursalesService(ISucursalesRepository sucursalesRepository)
        {
            this.sucursalesRepository = sucursalesRepository;
        }

        public async Task<List<Sucursales>> ListarSucursales(Int64 pnivel, Int64? pparent)
        {
            return await sucursalesRepository.ListarSucursales(pnivel,pparent);
        }

    }
}
