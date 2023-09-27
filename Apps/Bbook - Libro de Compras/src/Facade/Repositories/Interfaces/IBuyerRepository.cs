using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Repositories.Interfaces
{
    public interface IBuyerRepository
    {
        #region Metodos Principales
        //Obtener todas las sucursales
        public DTO<Buyer>.Request GetAllBuyers();
        //Crear o actualizar una sucursal
        public Task<DTO<Buyer>> LoadDataBuyer();
        #endregion
        #region Metodos Extras
        public PruebaInterna DeleteBbook_Buyer();
        #endregion
    }
}
