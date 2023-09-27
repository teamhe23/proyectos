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
    public class MetodoDistribucionService : IMetodoDistribucionService
    {
        private readonly IMetodoDistribucionRepository metodoDistribucionRepository;

        public MetodoDistribucionService(IMetodoDistribucionRepository metodoDistribucionRepository)
        {
            this.metodoDistribucionRepository = metodoDistribucionRepository;
        }

        public async Task<List<MetodoDistribucion>> listardistribucion()
        {
            return await metodoDistribucionRepository.listardistribucion();
        }
    }
}
