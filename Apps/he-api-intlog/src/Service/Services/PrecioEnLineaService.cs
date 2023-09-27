using Domain.Models;
using Domain.Models.Filters;
using Domain.Repositories;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class PrecioEnLineaService : IPrecioEnLineaService
    {
        private readonly IPrecioEnLineaRepository precioEnLineaRepository;

        public PrecioEnLineaService(IPrecioEnLineaRepository precioEnLineaRepository)
        {
            this.precioEnLineaRepository = precioEnLineaRepository;
        }

        public async Task<PrecioEnLineaResponse> registrar_precio_en_linea([FromBody] PrecioEnLineaRequest pe)
        {

            return await precioEnLineaRepository.registrar_precio_en_linea(pe);
        }

        public async Task<List<BuscarPrecioEnLinea>> ListarPrecioEnLinea(PrecioEnLineaFilterGet filtro)
        {
            return await precioEnLineaRepository.ListarPrecioEnLinea(filtro);
        }
    }
}
