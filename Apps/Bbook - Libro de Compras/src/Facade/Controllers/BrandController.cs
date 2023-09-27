using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models;
using IntegracionBbook.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static IntegracionBbook.Models.PruebaInternaDTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IntegracionBbook.Controllers
{
    [Route("/brands")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandRepository _brandRepository;
        public BrandController(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }
        // GET: api/<BrandController>
        [HttpGet("SP")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public DTO<Brand>.Request GetDataBrand()
        {
            return _brandRepository.GetAllBrands();
        }

        [HttpDelete]
        [ApiExplorerSettings(IgnoreApi = true)]
        public PruebaInterna DeleteBbook_Brand()
        {
            return _brandRepository.DeleteBbook_Brand();
        }

        // get api/<Brandcontroller>/5
        [HttpGet]
        public async Task<DTO<Brand>> LoadDataStoreAsync()
        {
            return await _brandRepository.LoadDataBrand();
        }
    }
}
