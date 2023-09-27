using Data.Oracle.Helpers;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace Facade.Controllers
{
    [Route("api/prioridaddeprecio")]
    [ApiController]
    public class PrioridadDePrecioController : ControllerBase
    {
        private readonly IPrioridadDePrecioService tipoPrecioService;

        public PrioridadDePrecioController(IPrioridadDePrecioService tipoPrecioService)
        {
            this.tipoPrecioService = tipoPrecioService;
        }

        [HttpGet]
        public async Task<IActionResult> ListarTipoPrecio()
        {
            try
            {
                return Ok(await tipoPrecioService.ListarTipoPrecio());
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
