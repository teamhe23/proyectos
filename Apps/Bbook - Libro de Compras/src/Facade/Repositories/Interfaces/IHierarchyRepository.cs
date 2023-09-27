using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models;
using IntegracionBbook.Models.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IntegracionBbook.Models.Hierarchy.HierarchyPost;
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Repositories.Interfaces
{
    public interface IHierarchyRepository
    {
        #region Metodos Principales
        //Obtener todas las dimensiones
        public HierarchyPostRequest GetAllHierarchies();
        //Crear o actualizar una dimension
        public Task<HierarchyDTO> LoadDataHierarchy();
        #endregion
        #region Metodos Extras
        public PruebaInterna DeleteBbook_Hierarchy();
        #endregion
    }
}
