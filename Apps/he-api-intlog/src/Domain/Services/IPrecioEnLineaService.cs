using Domain.Models;
using Domain.Models.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IPrecioEnLineaService
    {
        Task<List<BuscarPrecioEnLinea>> ListarPrecioEnLinea([FromBody] PrecioEnLineaFilterGet filtro);

        Task<PrecioEnLineaResponse> registrar_precio_en_linea([FromBody] PrecioEnLineaRequest pe);
    }
}
