using Data.Oracle.Helpers;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Service.Services;
using System.ComponentModel.DataAnnotations;

namespace Facade.Controllers
{
    [Route("api/tipoordencompra")]
    [ApiController]

    public class TipoOrdenCompraController : ControllerBase
    {
        private readonly ITipoOrdenCompraService tipoOrdenCompraService;

        public TipoOrdenCompraController(ITipoOrdenCompraService tipoOrdenCompraService)
        {
            this.tipoOrdenCompraService = tipoOrdenCompraService;
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> ListarTipoOrdenCompra([Required] string porigen)
        {

            try
            {
                return Ok(await tipoOrdenCompraService.ListarTipoOrdenCompra(porigen));
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
