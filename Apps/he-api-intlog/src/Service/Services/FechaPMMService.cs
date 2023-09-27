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
    public class FechaPMMService : IFechaPMMService
    {
        private readonly IFechaPMMRepository fechaPMMRepository;

        public FechaPMMService(IFechaPMMRepository fechaPMMRepository)
        {
            this.fechaPMMRepository = fechaPMMRepository;
        }
        public async Task<Fecha> ObtenerFecha()
        {
            return await fechaPMMRepository.ObtenerFecha();
        }
    }
}
