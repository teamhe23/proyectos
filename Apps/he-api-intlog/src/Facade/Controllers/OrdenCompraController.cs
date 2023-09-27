using Data.Oracle.Helpers;
using Domain.Models;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Service.Services;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace Facade.Controllers
{
    [Route("api/ordencompra")]
    [ApiController]
    public class OrdenCompraController : ControllerBase
    {
        private readonly IOrdenCompraService ordenCompraService;

        public OrdenCompraController(IOrdenCompraService ordenCompraService)
        {
            this.ordenCompraService = ordenCompraService;
        }


        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar_oc([FromBody] OrdenCompraRequest ocr)
        {
            try
            {
                return Ok(await ordenCompraService.Registrar_oc(ocr));
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

        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar_oc([Required] string finicial ,
                                                   [Required] string ffinal,
                                                   int? vpc_tech_key,
                                                   int? pmg_po_number
                                                    )
        {
            try
            {
                return Ok(await ordenCompraService.Buscar_oc(finicial, ffinal,
                                                             vpc_tech_key,
                                                             pmg_po_number
                                                             ));
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
