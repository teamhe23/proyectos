using Data.Oracle.Helpers;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Service.Services;

namespace Facade.Controllers
{
    [Route("api/proveedor")]
    [ApiController]
    public class ProveedorController : ControllerBase
    {
        private readonly IProveedorService ProveedorService;

        public ProveedorController(IProveedorService ProveedorService)
        {
            this.ProveedorService = ProveedorService;
        }

        [HttpGet("buscarproveedor")]
        public async Task<IActionResult> BuscarProveedor([FromQuery(Name = "pfiltro")] string? pFiltro = null)
        {

            try
            {
                return Ok(await ProveedorService.BuscarProveedor(pFiltro));
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
