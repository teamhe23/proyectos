﻿using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IProveedorRepository
    {
        Task<List<Proveedor>> BuscarProveedor(string pFiltro);       
    }
}
