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
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository productoRepository;

        public ProductoService(IProductoRepository productoRepository)
        {
            this.productoRepository = productoRepository;
        }

        public async Task<Producto> buscar_producto_oc(int metodo_distribucion, int vpc_tech_key, int org_lvl_child, string prd_lvl_number)
        {
            return await productoRepository.buscar_producto_oc(metodo_distribucion, vpc_tech_key, org_lvl_child, prd_lvl_number);
        }

        public async Task<ProductoPrecio?> ObtenerProductoPrecio(string codigo)
        {
            return await productoRepository.ObtenerProductoPrecio(codigo);
        }
    }
}
