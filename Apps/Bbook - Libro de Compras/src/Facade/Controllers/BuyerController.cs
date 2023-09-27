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
    [Route("/buyers")]
    [ApiController]
    public class BuyerController : ControllerBase
    {
        private readonly IBuyerRepository _buyerRepository;
        public BuyerController(IBuyerRepository buyerRepository)
        {
            _buyerRepository = buyerRepository;
        }
        [HttpGet("SP")] //Sin Procesar
        [ApiExplorerSettings(IgnoreApi = true)]
        public DTO<Buyer>.Request GetDataBuyer()
        {
            return _buyerRepository.GetAllBuyers();
            //TODO: Hacer el insert a la tabla historica
        }
        [HttpDelete]
        [ApiExplorerSettings(IgnoreApi = true)]
        public PruebaInterna DeleteBbook_Buyer()
        {
            return _buyerRepository.DeleteBbook_Buyer();
        }

        [HttpGet]
        public async Task<DTO<Buyer>> LoadDataBuyerAsync()
        {
            return await _buyerRepository.LoadDataBuyer();
        }
    }
}
