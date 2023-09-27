using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models;
using IntegracionBbook.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Controllers
{
    [Route("/comex")]
    [ApiController]
    public class ComexController : ControllerBase
    {
        private readonly IComexRepository _comexRepository;
        public ComexController(IComexRepository comexRepository)
        {
            _comexRepository = comexRepository;
        }
        // GET: api/<ComexController>
        [HttpGet("SP")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public DTO<Comex>.Request GetDataComex()
        {
            return _comexRepository.GetAllComexs();
        }

        [HttpDelete]
        [ApiExplorerSettings(IgnoreApi = true)]
        public PruebaInterna DeleteBbook_Comex()
        {
            return _comexRepository.DeleteBbook_Comex();
        }

        // get api/<Comexcontroller>/5
        [HttpGet]
        public async Task<DTO<Comex>> LoadDataComex()
        {
            return await _comexRepository.LoadDataComex();
        }
    }
}
