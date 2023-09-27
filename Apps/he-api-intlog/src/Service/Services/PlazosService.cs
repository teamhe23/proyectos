using Domain.Models;
using Domain.Repositories;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Services
{
    public class PlazosService : IPlazosService
    {
        private readonly IPlazosRepository plazosRepository;

        public PlazosService(IPlazosRepository plazosRepository)
        {
            this.plazosRepository = plazosRepository;
        }

        public async Task<List<Plazos>> ListarPlazos(string pTipOC)
        {
            return await plazosRepository.ListarPlazos(pTipOC);
        }

    }
}
