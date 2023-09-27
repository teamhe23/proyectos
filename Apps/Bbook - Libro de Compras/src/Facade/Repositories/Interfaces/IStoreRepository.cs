using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Repositories.Interfaces
{
    public interface IStoreRepository
    {
        #region Metodos Principales
        //Obtener todas las sucursales
        public DTO<Store>.Request GetAllStores(bool subproceso);
        //Crear o actualizar una sucursal
        public Task<DTO<Store>> LoadDataStore();
        #endregion
        #region Metodos Extras
        public PruebaInterna DeleteBbook_Store();
        #endregion
    }
}
