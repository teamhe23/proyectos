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
    public class CompradorService : ICompradorService
    {
        private readonly ICompradorRepository compradorRepository;

        public CompradorService(ICompradorRepository compradorRepository)
        {
            this.compradorRepository = compradorRepository;
        }

        public async Task<List<Comprador>> listarcompradores()
        {
            return await compradorRepository.listarcompradores();
        }
    }
}
