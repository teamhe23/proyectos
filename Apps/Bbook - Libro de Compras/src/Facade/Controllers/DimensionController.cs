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
    [Route("/dimensions")]
    [ApiController]
    public class DimensionController : Controller
    {
        private readonly IDimensionRepository _DimensionRepository;
        public DimensionController(IDimensionRepository DimensionRepository)
        {
            _DimensionRepository = DimensionRepository;
        }

        // GET: api/<DimensionController>
        [HttpGet("SP")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public DTO<Dimension>.Request GetDataDimension()
        {
            return _DimensionRepository.GetAllDimensions();
        }

        [HttpDelete]
        [ApiExplorerSettings(IgnoreApi = true)]
        public PruebaInterna DeleteBbook_Dimension()
        {
            return _DimensionRepository.DeleteBbook_Dimension();
        }

        // get api/<Dimensioncontroller>/5
        [HttpGet]
        public async Task<DTO<Dimension>> LoadDataStoreAsync()
        {
            return await _DimensionRepository.LoadDataDimension();
        }
    }
}
