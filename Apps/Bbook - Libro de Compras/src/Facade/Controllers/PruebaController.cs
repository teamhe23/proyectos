using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IntegracionBbook.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PruebaController : ControllerBase
    {
        // GET: api/<PruebaController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "hola", "prueba" };
        }

        [ApiExplorerSettings(IgnoreApi = false)]
        [HttpGet("probando")]
        public IEnumerable<string> Getprueba()
        {
            return new string[] { "hola", "prueba" };
        }
    }
}
