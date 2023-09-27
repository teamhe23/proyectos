using Domain.Models;
using Domain.Repositories;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class TiendaService :ITiendaService
    {
        private readonly ITiendaRepository tiendaRepository ;

        public TiendaService(ITiendaRepository tiendaRepository)
        {
            this.tiendaRepository = tiendaRepository;
        }

        public async Task<List<Tienda>> ListarTiendas(int solo_cd)
        {
            return await tiendaRepository.ListarTiendas(solo_cd);
        }


    }


}
