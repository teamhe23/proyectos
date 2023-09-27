using Data.Oracle.Helpers;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Service.Services;

namespace Facade.Controllers
{
    [Route("api/cadena")]
    [ApiController]
    public class CadenaController : ControllerBase
    {
        private readonly ICadenaPrecioService cadenaService;

        public CadenaController(ICadenaPrecioService cadenaService)
        {
            this.cadenaService = cadenaService;
        }

        [HttpGet]
        public async Task<IActionResult> ListarCadena()
        {
            try
            {
                return Ok(await cadenaService.ListarCadena());
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
