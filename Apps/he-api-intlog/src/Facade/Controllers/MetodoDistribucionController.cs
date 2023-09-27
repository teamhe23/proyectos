using Data.Oracle.Helpers;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Service.Services;

namespace Facade.Controllers
{
    [Route("api/metododistribucion")]
    [ApiController]
    public class MetodoDistribucionController : ControllerBase
    {
        private readonly IMetodoDistribucionService metodoDistribucionService;

        public MetodoDistribucionController(IMetodoDistribucionService metodoDistribucionService)
        {
            this.metodoDistribucionService = metodoDistribucionService;
        }

        [HttpGet]
        public async Task<IActionResult> listardistribucion()
        {
            try
            {
                return Ok(await metodoDistribucionService.listardistribucion());
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
