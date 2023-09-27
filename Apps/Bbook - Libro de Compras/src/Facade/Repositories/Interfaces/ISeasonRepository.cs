using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Repositories.Interfaces
{
    public interface ISeasonRepository
    {
        #region Metodos Principales
        //Obtener todas las temporadas
        public DTO<Season>.Request GetAllSeasons();
        //Crear o actualizar una temporada
        public Task<DTO<Season>> LoadDataSeason();
        #endregion
        #region Metodos Extras
        public PruebaInterna DeleteBbook_Season();
        #endregion
    }
}
