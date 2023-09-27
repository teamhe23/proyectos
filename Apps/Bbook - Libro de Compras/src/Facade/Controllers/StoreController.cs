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
    [Route("/stores")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IStoreRepository _storeRepository;
        public StoreController(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }
        [HttpGet("SP")] //Sin Procesar
        [ApiExplorerSettings(IgnoreApi = true)]
        public DTO<Store>.Request GetDataStore()
        {
            return _storeRepository.GetAllStores(false);
            //TODO: Hacer el insert a la tabla historica
        }
        [HttpDelete]
        [ApiExplorerSettings(IgnoreApi = true)]
        public PruebaInterna DeleteBbook_Store()
        {
            return _storeRepository.DeleteBbook_Store();
        }

        [HttpGet]
        public async Task<DTO<Store>> LoadDataStoreAsync()
        {
            return await _storeRepository.LoadDataStore();
        }
    }
}
