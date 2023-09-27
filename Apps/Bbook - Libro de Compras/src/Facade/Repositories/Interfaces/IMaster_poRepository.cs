using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Repositories.Interfaces
{
    public interface IMaster_poRepository
    {
        #region Metodos Principales
        //Obtener todas las marcas
        public DTO<Master_po>.Request GetAllMaster_pos(bool subproceso);
        //Crear o actualizar una marca
        public Task<DTO<Master_po>> LoadDataMaster_po();
        #endregion
        #region Metodos Extras
        public PruebaInterna DeleteBbook_Master_po();
        #endregion
    }
}
