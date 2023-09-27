using Data.Oracle.Helpers;
using Domain.Models;
using Domain.Models.Filters;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Service.Services;
using System.ComponentModel.DataAnnotations;

namespace Facade.Controllers
{
    [Route("api/precio")]
    [ApiController]
    public class PrecioController : ControllerBase
    {
        private readonly IPrecioService precioProgramadoService;

        public PrecioController(IPrecioService precioProgramadoService)
        {
            this.precioProgramadoService = precioProgramadoService;
        }

        [HttpPost("programado")]
        public async Task<IActionResult> GuardarProgramado([FromBody]PrecioProgramadoRequest modelo)
        {
            try
            {
                return Ok(await precioProgramadoService.GuardarProgramado(modelo));
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

        [HttpGet("programado")]
        public async Task<IActionResult> ListarProgramado([FromQuery]PrecioProgramadoFilterGet filtro)
        {

            try
            {
                if (string.IsNullOrEmpty(filtro.fecha_ini) || string.IsNullOrEmpty(filtro.fecha_fin))
                {
                    return BadRequest(new { mensaje = "Los parámetros fechainicio y fechafin son obligatorios." });
                }

                return Ok(await precioProgramadoService.ListarProgramado(filtro));
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
