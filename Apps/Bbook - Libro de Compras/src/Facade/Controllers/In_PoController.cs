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
    [Route("/in-po")]
    [ApiController]
    public class In_PoController : ControllerBase
    {
        private readonly IIn_poRepository _in_poRepository;

        public In_PoController(IIn_poRepository in_poRepository)
        {
            _in_poRepository = in_poRepository;
        }

        // GET: api/<In_poController>
        [HttpGet]
        public async Task<DTO<Out_Po>> LoadDataOut_po()
        {
            return await _in_poRepository.SendDataOut_po();
        }

        [HttpDelete]
        [ApiExplorerSettings(IgnoreApi = true)]
        public PruebaInternaDTO DeleteBbook_In_po()
        {
            return _in_poRepository.DeleteBbook_In_po();
        }

        // get api/<In_pocontroller>/5
        [HttpPost]
        public DTOUnitario<In_Po>.Response LoadDataIn_po([FromBody] DTOUnitario<In_Po>.Request in_PoRequest)
        {
            DTOUnitario<In_Po>.Response in_PoResponse = _in_poRepository.LoadDataIn_po(in_PoRequest);
            Response.StatusCode = in_PoResponse.statusCode;
            return in_PoResponse;
        }

        
    }
}
