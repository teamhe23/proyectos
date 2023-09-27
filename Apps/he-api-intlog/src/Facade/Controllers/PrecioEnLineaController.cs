using Data.Oracle.Helpers;
using Domain.Models.Filters;
using Domain.Models;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Service.Services;
using System.ComponentModel.DataAnnotations;

namespace Facade.Controllers
{
    [Route("api/precio/enlinea")]
    [ApiController]
    public class PrecioEnLineaController : ControllerBase
    {
        private readonly IPrecioEnLineaService precioEnLineaService;

        public PrecioEnLineaController(IPrecioEnLineaService precioEnLineaService)
        {
            this.precioEnLineaService = precioEnLineaService;
        }

        [HttpPost]
        public async Task<IActionResult> registrar_precio_en_linea([FromBody] PrecioEnLineaRequest pe)
        {
            try
            {
                return Ok(await precioEnLineaService.registrar_precio_en_linea(pe));
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

        [HttpGet]
        public async Task<IActionResult> ListarPrecioEnLinea([FromQuery] PrecioEnLineaFilterGet filtro)
        {
            try
            {
                if (string.IsNullOrEmpty(filtro.fechainicio) || string.IsNullOrEmpty(filtro.fechafin))
                {
                    return BadRequest(new { mensaje = "Los parámetros fechainicio y fechafin son obligatorios." });
                }

                return Ok(await precioEnLineaService.ListarPrecioEnLinea(filtro));
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
