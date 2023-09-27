using Facade.Models.In_Asn;
using IntegracionBbook.Api.Models.In_po;
using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Repositories.Interfaces
{
    public interface IIn_AsnRepository
    {
        #region Metodos Principales
        public DTOUnitario<In_Asn>.Response LoadDataIn_Asn(DTOUnitario<In_Asn>.Request in_AsnRequest);
        //public Task<DTO<Out_Po>> SendDataOut_po();
        #endregion
        #region Metodos Extras
        public PruebaInternaDTO DeleteBbook_In_Asn();
        #endregion
    }
}
