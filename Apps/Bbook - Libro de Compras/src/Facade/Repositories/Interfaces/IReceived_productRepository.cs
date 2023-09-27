using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Repositories.Interfaces
{
    public interface IReceived_productRepository
    {
        #region Metodos Principales
        //Obtener todas las marcas
        public DTO<Received_product>.Request GetAllReceived_products();
        //Crear o actualizar una marca
        public Task<DTO<Received_product>> LoadDataReceived_product();
        #endregion
        #region Metodos Extras
        public PruebaInterna DeleteBbook_Received_product();
        #endregion
    }
}
