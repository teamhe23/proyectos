using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IntegracionBbook.Models.Brand;
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Repositories.Interfaces
{
    public interface IBrandRepository
    {
        #region Metodos Principales
        //Obtener todas las marcas
        public DTO<Brand>.Request GetAllBrands();
        //Crear o actualizar una marca
        public Task<DTO<Brand>> LoadDataBrand();
        #endregion
        #region Metodos Extras
        public PruebaInterna DeleteBbook_Brand();
        #endregion
    }
}
