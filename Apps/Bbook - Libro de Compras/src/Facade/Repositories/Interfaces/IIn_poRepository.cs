using IntegracionBbook.Api.Models.In_po;
using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Repositories.Interfaces
{
    public interface IIn_poRepository
    {
        #region Metodos Principales
        //Obtener todas las ordenes de compra
        //public DTOUnitario<In_Po>.Request GetAllIn_pos();
        //Crear o actualizar una orden de compra
        public DTOUnitario<In_Po>.Response LoadDataIn_po(DTOUnitario<In_Po>.Request in_PoRequest);
        public Task<DTO<Out_Po>> SendDataOut_po();
        #endregion
        #region Metodos Extras
        public PruebaInternaDTO DeleteBbook_In_po();
        #endregion
    }
}
