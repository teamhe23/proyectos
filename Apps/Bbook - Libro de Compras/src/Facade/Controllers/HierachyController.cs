using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models;
using IntegracionBbook.Models.Hierarchy;
using IntegracionBbook.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Controllers
{
    [Route("/hierarchies")]
    [ApiController]
    public class HierarchyController : Controller
    {
        private readonly IHierarchyRepository _HierarchyRepository;
        public HierarchyController(IHierarchyRepository HierarchyRepository)
        {
            _HierarchyRepository = HierarchyRepository;
        }

        // GET: api/<HierarchyController>
        [HttpGet("SP")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public HierarchyPost.HierarchyPostRequest GetDataHierarchy()
        {
            return _HierarchyRepository.GetAllHierarchies();
        }

        [HttpDelete]
        [ApiExplorerSettings(IgnoreApi = true)]
        public PruebaInterna DeleteBbook_Hierarchy()
        {
            return _HierarchyRepository.DeleteBbook_Hierarchy();
        }

        // get api/<Hierarchycontroller>/5
        [HttpGet]
        public async Task<HierarchyDTO> LoadDataStoreAsync()
        {
            return await _HierarchyRepository.LoadDataHierarchy();
        }
    }
}
