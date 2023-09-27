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
    public class ProveedorService : IProveedorService
    {
        private readonly IProveedorRepository ProveedorRepository;

        public ProveedorService(IProveedorRepository metodoProveedorRepository)
        {
            this.ProveedorRepository = metodoProveedorRepository;
        }

        public async Task<List<Proveedor>> BuscarProveedor(string pFiltro)
        {
            return await ProveedorRepository.BuscarProveedor(pFiltro);    
        }        
    }
}
