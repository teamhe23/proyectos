using Data.Oracle.Helpers;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Service.Services;
using System.ComponentModel.DataAnnotations;

namespace Facade.Controllers
{
    [Route("api/puerto")]
    [ApiController]
    public class PuertoController : ControllerBase
    {
        private readonly IPuertoService puertoService;

        public PuertoController(IPuertoService puertoService)
        {
            this.puertoService = puertoService;
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> ListarPuerto([Required] int psucursal)
        {

            try
            {
                return Ok(await puertoService.ListarPuerto(psucursal));
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
