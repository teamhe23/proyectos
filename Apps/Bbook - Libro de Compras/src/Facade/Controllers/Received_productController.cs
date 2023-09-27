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
    [Route("/received-products")]
    [ApiController]
    public class Received_productController : ControllerBase
    {
        private readonly IReceived_productRepository _received_productRepository;
        public Received_productController(IReceived_productRepository received_productRepository)
        {
            _received_productRepository = received_productRepository;
        }
        // GET: api/<Received_productController>
        [HttpGet("SP")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public DTO<Received_product>.Request GetDataReceived_product()
        {
            return _received_productRepository.GetAllReceived_products();
        }

        [HttpDelete]
        [ApiExplorerSettings(IgnoreApi = true)]
        public PruebaInterna DeleteBbook_Received_product()
        {
            return _received_productRepository.DeleteBbook_Received_product();
        }

        // get api/<Received_productcontroller>/5
        [HttpGet]
        public async Task<DTO<Received_product>> LoadDataStoreAsync()
        {
            return await _received_productRepository.LoadDataReceived_product();
        }
    }
}
