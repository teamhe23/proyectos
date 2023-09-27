using IntegracionBbook.Api.Models;
using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Api.Repositories.Interfaces
{
    public interface IProduct_modificationRepository
    {
        #region Metodos Principales
        //Obtener todas las ordenes de compra
        //public DTOUnitario<In_Po>.Request GetAllIn_pos();
        //Crear o actualizar una orden de compra
        public DTO<Product_modification>.Response LoadDataProduct_Modification(DTO<Product_modification>.Request product_ModificationRequest);
        #endregion
        #region Metodos Extras
        public PruebaInternaDTO DeleteBbook_Product_Modification();
        #endregion
    }
}
