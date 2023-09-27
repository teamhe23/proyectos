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
    public interface IPrecioService
    {
        Task<PrecioProgramadoResponse> GuardarProgramado([FromBody] PrecioProgramadoRequest pp);

        Task<List<PrecioProgramadoBuscar>> ListarProgramado([FromQuery] PrecioProgramadoFilterGet filtro);
    }
}
