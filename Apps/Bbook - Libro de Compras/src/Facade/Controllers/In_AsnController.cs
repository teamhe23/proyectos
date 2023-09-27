using Facade.Models.In_Asn;
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
    [Route("/in-asn")]
    [ApiController]
    public class In_AsnController : ControllerBase
    {
        private readonly IIn_AsnRepository _in_AsnRepository;

        public In_AsnController(IIn_AsnRepository in_AsnRepository)
        {
            _in_AsnRepository = in_AsnRepository;
        }

        // GET: api/<In_poController>
        [HttpGet]
        public async Task<DTO<Out_Po>> LoadDataOut_asn()
        {
            return null;
            //return await _in_AsnRepository.SendDataOut_po();
        }

        [HttpDelete]
        [ApiExplorerSettings(IgnoreApi = true)]
        public PruebaInternaDTO DeleteBbook_In_asn()
        {
            return _in_AsnRepository.DeleteBbook_In_Asn();
        }

        // get api/<In_pocontroller>/5
        [HttpPost]
        public DTOUnitario<In_Asn>.Response LoadDataIn_asn([FromBody] DTOUnitario<In_Asn>.Request in_AsnRequest)
        {
            DTOUnitario<In_Asn>.Response in_AsnResponse = _in_AsnRepository.LoadDataIn_Asn(in_AsnRequest);
            Response.StatusCode = in_AsnResponse.statusCode;
            return in_AsnResponse;
        }

    }
}