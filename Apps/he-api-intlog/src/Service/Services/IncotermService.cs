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
    public class IncotermService: IIncotermService
    {
        private readonly IIncotermRepository incotermRepository;

        public IncotermService(IIncotermRepository incotermRepository)
        {
            this.incotermRepository = incotermRepository;
        }

        public async Task<List<Incoterm>> ListaIncoterm()
        {
            return await incotermRepository.ListaIncoterm();
        }

    }
}
