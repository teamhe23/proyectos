using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Repositories.Interfaces
{
    public interface IVendorRepository
    {
        #region Metodos Principales
        //Obtener todas los proveedores
        public DTO<Vendor>.Request GetAllVendors(bool subproceso);
        //Crear o actualizar un proveedor
        public Task<DTO<Vendor>> LoadDataVendor();
        #endregion
        #region Metodos Extras
        public PruebaInterna DeleteBbook_Vendor();
        #endregion
    }
}
