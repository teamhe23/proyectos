using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Repositories.Interfaces
{
    public interface IDimensionRepository
    {
        #region Metodos Principales
        //Obtener todas las dimensiones
        public DTO<Dimension>.Request GetAllDimensions();
        //Crear o actualizar una dimension
        public Task<DTO<Dimension>> LoadDataDimension();
        #endregion
        #region Metodos Extras
        public PruebaInterna DeleteBbook_Dimension();
        #endregion
    }
}
