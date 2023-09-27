using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IOrdenCompraService
    {
        Task<OrdenCompraResponse> Registrar_oc([FromBody] OrdenCompraRequest ocr);

        Task<List<BuscarOC>> Buscar_oc(string finicial, string ffinal , 
                                       int? vpc_tech_key,
                                       int? pmg_po_number
                                        );
            

    }
}
