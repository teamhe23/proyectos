using Data.Oracle.Helpers;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Service.Services;

namespace Facade.Controllers
{
    [Route("api/comprador")]
    [ApiController]
    public class CompradorController : ControllerBase
    {
        private readonly ICompradorService compradorService;

        public CompradorController(ICompradorService compradorService)
        {
            this.compradorService = compradorService;
        }

        [HttpGet]
        public async Task<IActionResult> listarcompradores()
        {
            try
            {
                return Ok(await compradorService.listarcompradores());
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
