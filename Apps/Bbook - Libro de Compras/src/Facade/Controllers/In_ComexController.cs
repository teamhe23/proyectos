using Facade.Models.In_Asn;
using Facade.Models.In_Comex;
using IntegracionBbook.Api.Models.In_po;
using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models;
using IntegracionBbook.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace IntegracionBbook.Controllers
{
    [Route("/in-comex")]
    [ApiController]
    public class In_ComexController : ControllerBase
    {
        private readonly IIn_ComexRepository _in_ComexRepository;
        public In_ComexController(IIn_ComexRepository in_ComexRepository)
        {
            _in_ComexRepository = in_ComexRepository;
        }
        [HttpGet]
        public async Task<DTO<Out_Po>> LoadDataOut_In_Comex()
        {
            return null;
            //return await _in_AsnRepository.SendDataOut_po();
        }
        [HttpDelete]
        [ApiExplorerSettings(IgnoreApi = true)]
        public PruebaInternaDTO DeleteBbook_In_Comex()
        {
            return _in_ComexRepository.DeleteBbook_In_Comex();
        }

        
        [HttpPost]
        public DTOUnitario<In_Comex>.Response LoadDataIn_Comex([FromBody] DTOUnitario<In_Comex>.Request in_ComexRequest)
            {
            DTOUnitario<In_Comex>.Response in_ComexResponse = _in_ComexRepository.LoadDataIn_Comex(in_ComexRequest);
            Response.StatusCode = in_ComexResponse.statusCode;
            return in_ComexResponse;
        }
        
    }
}