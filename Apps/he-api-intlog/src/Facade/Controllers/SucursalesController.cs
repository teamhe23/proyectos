using Data.Oracle.Helpers;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Service.Services;
using System.ComponentModel.DataAnnotations;

namespace Facade.Controllers
{
    [Route("api/sucursales")]
    [ApiController]
    public class SucursalesController :ControllerBase
    {
        private readonly ISucursalesService sucursalesService;

        public SucursalesController(ISucursalesService sucursalesService)
        {
            this.sucursalesService = sucursalesService;
        }

        [HttpGet]
        public async Task<IActionResult> ListarSucursales([Required] Int64 pnivel, Int64? pparent)            
        {
            try
            {
               // return Ok(await sucursalesService.ListarSucursales(pnivel,pparent));

                var sucursales = await sucursalesService.ListarSucursales(pnivel, pparent);

                if (sucursales != null && sucursales.Count > 0)
                {
                    return Ok(sucursales);
                }
                else
                {
                    return NotFound("No existe el nivel en la sucursal");
                }

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
