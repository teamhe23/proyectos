using Domain.Models;
using Domain.Models.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IPrecioEnLineaRepository
    {
        Task<PrecioEnLineaResponse> registrar_precio_en_linea([FromBody] PrecioEnLineaRequest pe);
        Task<List<BuscarPrecioEnLinea>> ListarPrecioEnLinea([FromQuery] PrecioEnLineaFilterGet filtro);
    }
}
