using Domain.Models;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Service.Services;
using System.ComponentModel.DataAnnotations;
using Data.Oracle.Helpers;

namespace Facade.Controllers
{
    [Route("api/producto")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService productoService;

        public ProductoController(IProductoService productoService)
        {
            this.productoService = productoService;
        }

        [HttpGet("ordencompra")]
        public async Task<IActionResult> buscar_producto_oc([Required] int metodo_distribucion, [Required] int vpc_tech_key, [Required] int org_lvl_child, [Required] string prd_lvl_number)
        {
            
            try
            {
                return Ok(await productoService.buscar_producto_oc(metodo_distribucion, vpc_tech_key,org_lvl_child, prd_lvl_number));
            }
            catch (OracleException ex)
            {
                return BadRequest(new { mensaje = new ExceptionInternal(ex.Message).Message });  
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet("precio")]
        public async Task<IActionResult> ObtenerProductoPrecio([Required]string codigo)
        {
            try
            {
                var productos = await productoService.ObtenerProductoPrecio(codigo);

                if (productos == null)
                {
                    return NotFound("");
                }
                return Ok(productos);
            }
            catch (OracleException ex)
            {
                return BadRequest(new { mensaje = new ExceptionInternal(ex.Message).Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
