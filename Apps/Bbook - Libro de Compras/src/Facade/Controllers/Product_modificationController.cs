using IntegracionBbook.Api.Models;
using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Api.Repositories.Interfaces;
using IntegracionBbook.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Api.Controllers
{
    [Route("/product-modification")]
    [ApiController]
    public class Product_modificationController : ControllerBase
    {
        private readonly IProduct_modificationRepository _product_modificationRepository;

        public Product_modificationController(IProduct_modificationRepository product_modificationRepository)
        {
            _product_modificationRepository = product_modificationRepository;
        }

        [HttpDelete]
        [ApiExplorerSettings(IgnoreApi = true)]
        public PruebaInternaDTO DeleteBbook_Product_modification()
        {
            return _product_modificationRepository.DeleteBbook_Product_Modification();
        }

        // get api/<In_pocontroller>/5
        [HttpPost]
        public DTO<Product_modification>.Response LoadDataProduct_modification([FromBody] DTO<Product_modification>.Request product_ModificationRequest)
        {
            DTO<Product_modification>.Response product_ModificationResponse = _product_modificationRepository.LoadDataProduct_Modification(product_ModificationRequest);
            Response.StatusCode = product_ModificationResponse.statusCode;
            return product_ModificationResponse;
        }
    }
}
