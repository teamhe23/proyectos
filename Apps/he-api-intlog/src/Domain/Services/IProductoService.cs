using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IProductoService
    {
        Task<Producto> buscar_producto_oc(int metodo_distribucion, int vpc_tech_key, int org_lvl_child, string prd_lvl_number);
        Task<ProductoPrecio?> ObtenerProductoPrecio(string codigo);
    }
}
