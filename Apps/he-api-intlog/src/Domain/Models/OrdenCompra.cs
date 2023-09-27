using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{

    public class OrdenCompraRequest { 
    
        public OrdenCompra cabecera { get; set; }
        public List<OrdenCompraDet> detalle { get; set; }  

    }

    public class BuscarOC
    {
        
        public decimal pmg_po_number { get; set; }
        public string vendor_number { get; set; } = string.Empty;
        public string vendor_name { get; set; } = string.Empty;
        public string dmt_desc { get; set; }
        public string pmg_stat_name { get; set; }
        public string pmg_user { get; set; } = string.Empty;
        public DateTime pmg_entry_date { get; set; } 
        public string curr_code { get; set; } = string.Empty;
        public decimal total { get; set; }
    }

    public class OrdenCompraResponse
    {
        public decimal pmg_po_number { get; set; }
    }

    public class OrdenCompra
    {
       // -- audit_number    in tpimpocc.audit_number%type,
       public string sistema { get; set; } = string.Empty;      
       // -- pmg_po_number   in tpimpocc.pmg_po_number%type default 0,
    
        public string vendedor_number { get; set; } = string.Empty;

        public decimal org_lvl_number { get; set; }

        public int dmt_code { get; set; } 

        public string rct_date { get; set; } = string.Empty;

        public string pmg_buyer { get; set; } = string.Empty;

        public string cncl_date { get; set; } = string.Empty;

        //-- ext_po_number   in tpimpocc.pmg_ext_po_num%type,

        public string pmg_type_name { get; set; } = string.Empty;

        public decimal pmg_percent { get; set; }

        public string plazo { get; set; } = string.Empty;
        public decimal flgcp { get; set; }

        public string pmg_lc_number { get; set; } = string.Empty;
        public string flg_req_ins { get; set; } = string.Empty;
        public string shipping_date { get; set; } = string.Empty;
        public decimal id_puerto { get; set; }
        public string id_incoterm { get; set; } = string.Empty;
        public decimal contenedor_20 { get; set; }
        public decimal contenedor_40 { get; set; }
        public decimal contenedor_hc { get; set; }
        public decimal contenedor_lcl { get; set; }

        //public int IntExpPONumber { get; set; }
        //public int IntAuditNumberCab { get; set; }

    }

    public class OrdenCompraDet
    {
        //public decimal pmg_line_number { get; set; }
        public string prd_lvl_number { get; set; } = string.Empty;
        public decimal org_lvl_number { get; set; }
        public decimal dist_sell_qty { get; set; }
        public int dmt_code { get; set; } 

        //public string ext_po_number { get; set; } = string.Empty;
        public int prd_lvl_id { get; set; }
        public string cod_stl { get; set; } = string.Empty;
        
        //public decimal audit_number_cab { get; set; }
        public decimal pmg_sell_cost { get; set; }
    }

}
