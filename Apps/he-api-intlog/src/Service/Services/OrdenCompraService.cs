using Domain.Models;
using Domain.Repositories;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class OrdenCompraService : IOrdenCompraService
    {
        private readonly IOrdenCompraRepository ordenCompraRepository;

        public OrdenCompraService(IOrdenCompraRepository ordenCompraRepository)
        {
            this.ordenCompraRepository = ordenCompraRepository;
        }

        public async Task<OrdenCompraResponse>Registrar_oc([FromBody] OrdenCompraRequest ocr)
        {
            return await ordenCompraRepository.Registrar_oc(ocr);
        }

        public async Task<List<BuscarOC>>Buscar_oc(string finicial, string ffinal , 
                                                    int? vpc_tech_key,
                                                    int? pmg_po_number
                                                    )
        {
            return await ordenCompraRepository.Buscar_oc(finicial, ffinal ,
                                                         vpc_tech_key,
                                                         pmg_po_number
                                                         );
        }

    }
}
