using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Repositories.Interfaces
{
    public interface ISizeRepository
    {
        #region Metodos Principales
        //Obtener todas las tallas
        public DTO<Size>.Request GetAllSizes();
        //Crear o actualizar una talla
        public Task<DTO<Size>> LoadDataSize();
        #endregion
        #region Metodos Extras
        public PruebaInterna DeleteBbook_Size();
        #endregion
    }
}
