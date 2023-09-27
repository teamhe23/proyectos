using Data.Oracle.Helpers;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Service.Services;

namespace Facade.Controllers
{
    [Route("api/tienda")]
    [ApiController]
    public class TiendaController :ControllerBase
    {
        private readonly ITiendaService TiendaService;

        public TiendaController(ITiendaService TiendaService)
        {
            this.TiendaService = TiendaService;
        }
      
       [HttpGet("buscar")]
        public async Task<IActionResult> ListarTiendas(int solo_cd)
        {

            try
            {
                return Ok(await TiendaService.ListarTiendas(solo_cd));
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
