using Data.Oracle.Helpers;
using Domain.Models;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Service.Services;

namespace Facade.Controllers
{
    [Route("api/fechapmm")]
    [ApiController]
    public class FechaPMMController : ControllerBase
    {
        private readonly IFechaPMMService fechaPMMService;

        public FechaPMMController(IFechaPMMService fechaPMMService)
        {
            this.fechaPMMService = fechaPMMService;
        }

        [HttpGet]
        public async Task<IActionResult>  ObtenerFecha()
        {
            try
            {

               return Ok(await fechaPMMService.ObtenerFecha());                
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
