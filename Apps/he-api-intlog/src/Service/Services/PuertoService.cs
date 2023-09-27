using Domain.Models;
using Domain.Repositories;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Services
{
    public class PuertoService:IPuertoService
    {
        private readonly IPuertoRepository puertoRepository;

        public PuertoService(IPuertoRepository puertoRepository)
        {
            this.puertoRepository = puertoRepository;
        }

        public async Task<List<Puerto>> ListarPuerto(int psucursal)
        {
            return await puertoRepository.ListarPuerto(psucursal);
        }

    }
}
