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
    [Route("/sizes")]
    [ApiController]
    public class SizeController : Controller
    {
        private readonly ISizeRepository _SizeRepository;
        public SizeController(ISizeRepository SizeRepository)
        {
            _SizeRepository = SizeRepository;
        }

        // GET: api/<SizeController>
        [HttpGet("SP")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public DTO<Size>.Request GetDataSize()
        {
            return _SizeRepository.GetAllSizes();
        }

        [HttpDelete]
        [ApiExplorerSettings(IgnoreApi = true)]
        public PruebaInterna DeleteBbook_Size()
        {
            return _SizeRepository.DeleteBbook_Size();
        }

        // get api/<Sizecontroller>/5
        [HttpGet]
        public async Task<DTO<Size>> LoadDataStoreAsync()
        {
            return await _SizeRepository.LoadDataSize();
        }
    }
}
