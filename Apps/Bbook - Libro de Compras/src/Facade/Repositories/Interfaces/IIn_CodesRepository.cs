using IntegracionBbook.Api.Models.In_Codes;
using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Repositories.Interfaces
{
    public interface IIn_CodesRepository
    {
        #region Metodos Principales
        //Obtener todas los codigos de producto
        //public DTO<In_Codes>.Response GetAllIn_Codes();
        //Crear o actualizar uncodigo de producto
        public DTOUnitario<In_Codes>.Response LoadDataIn_Codes(DTOUnitario<In_Codes>.Request in_CodesRequest,ref PruebaInterna pruebaInterna);
        public Task<DTO<Out_Codes>> SendDataOut_Codes();
        #endregion
        #region Metodos Extras
        public PruebaInternaDTO DeleteBbook_In_Codes();
        #endregion
    }
}
