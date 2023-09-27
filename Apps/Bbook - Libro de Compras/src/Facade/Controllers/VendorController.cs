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
    [Route("/vendors")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly IVendorRepository _vendorRepository;
        public VendorController(IVendorRepository vendorRepository)
        {
            _vendorRepository = vendorRepository;
        }
        // GET: api/<VendorController>
        [HttpGet("SP")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public DTO<Vendor>.Request GetDataVendor()
        {
            return _vendorRepository.GetAllVendors(false);
        }

        [HttpDelete]
        [ApiExplorerSettings(IgnoreApi = true)]
        public PruebaInterna DeleteBbook_Vendor()
        {
            return _vendorRepository.DeleteBbook_Vendor();
        }

        // get api/<Vendorcontroller>/5
        [HttpGet]
        public async Task<DTO<Vendor>> LoadDataVendor()
        {
            return await _vendorRepository.LoadDataVendor();
        }
    }
}
