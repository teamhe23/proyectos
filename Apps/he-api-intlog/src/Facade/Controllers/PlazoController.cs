using Data.Oracle.Helpers;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Service.Services;

namespace Facade.Controllers
{
    [Route("api/plazo")]
    [ApiController]
    public class PlazoController :ControllerBase
    {
        private readonly IPlazosService plazosService;

        public PlazoController(IPlazosService plazosService)
        {
            this.plazosService = plazosService;
        }

        [HttpGet]
        public async Task<IActionResult> ListarPlazos([FromQuery(Name = "pfiltro")] string? pFiltro = null)
        {

            try
            {
                return Ok(await plazosService.ListarPlazos(pFiltro));
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
