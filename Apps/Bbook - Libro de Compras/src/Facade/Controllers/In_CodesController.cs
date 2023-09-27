using IntegracionBbook.Models;
using IntegracionBbook.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Api.Models.In_Codes;
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Controllers
{
    [Route("/in-codes")]
    [ApiController]
    public class In_CodesController : ControllerBase
    {
        private readonly IIn_CodesRepository _in_codesRepository;
        private readonly ICommonRepository _commonRepository; //Interface de metodos comunes
        public In_CodesController(IIn_CodesRepository in_codesRepository, ICommonRepository commonRepository)
        {
            _in_codesRepository = in_codesRepository;
            _commonRepository = commonRepository;
        }

        //GET: api/<In_poController>
        [HttpGet]
        public async Task<DTO<Out_Codes>> LoadDataOut_Codes()
        {
            return await _in_codesRepository.SendDataOut_Codes();
        }

        [HttpDelete]
        [ApiExplorerSettings(IgnoreApi = true)]
        public PruebaInternaDTO DeleteBbook_In_po()
        {
            return _in_codesRepository.DeleteBbook_In_Codes();
        }

        // get api/<In_pocontroller>/5
        [HttpPost]
        public async Task<DTOUnitario<In_Codes>.Response> LoadDataIn_CodesAsync([FromBody]DTOUnitario<In_Codes>.Request in_codesRequest)
        {
            int proceso = 0;
            PruebaInterna pruebaInterna = new PruebaInterna();
            DTOUnitario<In_Codes>.Response response = _in_codesRepository.LoadDataIn_Codes(in_codesRequest,ref pruebaInterna);
            Response.StatusCode = response.statusCode;
            _commonRepository.Bbook_history("Bbook_In_codes", JsonConvert.SerializeObject(in_codesRequest),
                        JsonConvert.SerializeObject(response), JsonConvert.SerializeObject(pruebaInterna));
            return response;
        }
    }
}
