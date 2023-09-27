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
    [Route("/seasons")]
    [ApiController]
    public class SeasonController : Controller
    {
        private readonly ISeasonRepository _SeasonRepository;
        public SeasonController(ISeasonRepository SeasonRepository)
        {
            _SeasonRepository = SeasonRepository;
        }

        // GET: api/<SeasonController>
        [HttpGet("SP")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public DTO<Season>.Request GetDataSeason()
        {
            return _SeasonRepository.GetAllSeasons();
        }

        [HttpDelete]
        [ApiExplorerSettings(IgnoreApi = true)]
        public PruebaInterna DeleteBbook_Season()
        {
            return _SeasonRepository.DeleteBbook_Season();
        }

        // get api/<Seasoncontroller>/5
        [HttpGet]
        public async Task<DTO<Season>> LoadDataStore()
        {
            return await _SeasonRepository.LoadDataSeason();
        }
    }
}
