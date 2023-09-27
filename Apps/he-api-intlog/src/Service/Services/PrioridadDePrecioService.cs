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
    public class PrioridadDePrecioService : IPrioridadDePrecioService
    {
        private readonly IPrioridadDePrecioRepository tipoPrecioRepository;

        public PrioridadDePrecioService (IPrioridadDePrecioRepository tipoPrecioRepository)
        {
            this.tipoPrecioRepository = tipoPrecioRepository;
        }
        public async Task<List<PrioridadDePrecio>> ListarTipoPrecio()
        {
            return await tipoPrecioRepository.ListarTipoPrecio();
        }
    }
}
