using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Repositories.Interfaces
{
    public interface IComexRepository
    {
        #region Metodos Principales
        //Obtener todas las marcas
        public DTO<Comex>.Request GetAllComexs();
        //Crear o actualizar una marca
        public Task<DTO<Comex>> LoadDataComex();
        #endregion
        #region Metodos Extras
        public PruebaInterna DeleteBbook_Comex();
        #endregion
    }
}
