using Data.Oracle.Helpers;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Service.Services;

namespace Facade.Controllers
{
    [Route("api/incoterm")]
    [ApiController]
    public class IncotermController : ControllerBase
    {
        private readonly IIncotermService incotermService;

        public IncotermController(IIncotermService incotermService)
        {
            this.incotermService = incotermService;
        }

        [HttpGet]
        public async Task<IActionResult> ListaIncoterm()
        {
            try
            {
                return Ok(await incotermService.ListaIncoterm());
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
