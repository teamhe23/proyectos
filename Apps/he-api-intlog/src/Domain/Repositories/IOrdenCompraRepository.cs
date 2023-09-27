using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Core;
using Microsoft.AspNetCore.Mvc;

namespace Domain.Repositories
{
    public interface IOrdenCompraRepository
    {
        Task<OrdenCompraResponse> Registrar_oc([FromBody] OrdenCompraRequest ocr);
        Task<List<BuscarOC>> Buscar_oc(string finicial, string ffinal, 
                                        int? vpc_tech_key, 
                                        int? pmg_po_number
                                        );
    }
}
