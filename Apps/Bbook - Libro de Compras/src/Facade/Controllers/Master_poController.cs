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
    [Route("/master-po")]
    [ApiController]
    public class Master_poController : ControllerBase
    {
        private readonly IMaster_poRepository _master_poRepository;
        public Master_poController(IMaster_poRepository master_poRepository)
        {
            _master_poRepository = master_poRepository;
        }

        // GET: api/<Master_poController>
        [HttpGet("SP")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public DTO<Master_po>.Request GetDataMaster_po()
        {
            return _master_poRepository.GetAllMaster_pos(false);
        }

        [HttpDelete]
        [ApiExplorerSettings(IgnoreApi = true)]
        public PruebaInterna DeleteBbook_Master_po()
        {
            return _master_poRepository.DeleteBbook_Master_po();
        }

        // get api/<Master_pocontroller>/5
        [HttpGet]
        public async Task<DTO<Master_po>> LoadDataStoreAsync()
        {
            return await _master_poRepository.LoadDataMaster_po();
        }
    }
}
