using Facade.Models.In_Comex;
using IntegracionBbook.Api.Models.In_po;
using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace IntegracionBbook.Repositories.Interfaces
{
    public interface IIn_ComexRepository
    {
        #region Metodos Principales
        public DTOUnitario<In_Comex>.Response LoadDataIn_Comex(DTOUnitario<In_Comex>.Request in_AsnRequest);
        //public Task<DTO<Out_Po>> SendDataOut_po();
        #endregion
        #region Metodos Extras
        public PruebaInternaDTO DeleteBbook_In_Comex();
        #endregion
    }
}
