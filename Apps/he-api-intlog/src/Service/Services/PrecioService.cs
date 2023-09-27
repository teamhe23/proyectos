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
    public class PrecioService :IPrecioService
    {
        private readonly IPrecioRepository precioProgramadoRepository;

        public PrecioService(IPrecioRepository precioProgramadoRepository)
        {
            this.precioProgramadoRepository = precioProgramadoRepository;
        }

        public async Task<PrecioProgramadoResponse> GuardarProgramado(PrecioProgramadoRequest modelo)
        {            
            return await precioProgramadoRepository.GuardarProgramado(modelo);
        }

        public async Task<List<PrecioProgramadoBuscar>> ListarProgramado(PrecioProgramadoFilterGet filtro)
        {
            return await precioProgramadoRepository.ListarProgramado(filtro);
        }

    }
}
