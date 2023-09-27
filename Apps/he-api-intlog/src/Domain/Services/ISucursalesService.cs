using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface ISucursalesService
    {
        Task<List<Sucursales>> ListarSucursales(Int64 pnivel, Int64? pparent);
    }
}
