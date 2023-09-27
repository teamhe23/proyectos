using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{

    public class PrecioEnLineaRequest
    {
        public PrecioEnLinea cabecera { get; set; }
        public List<PrecioEnLineaDet> detalle { get; set; }

    }

    public class PrecioEnLinea
    {      
        public Int64 sucursal { get; set; }
        public string usuario { get; set; } = string.Empty;
        public string userhost { get; set; } = string.Empty;

    }

    public class PrecioEnLineaDet
    {
        public string producto { get; set; } = string.Empty;
        public Int64 precio { get; set; }
        public Int64 indevento { get; set; }        
        public Int64 qtyiniran1 { get; set; }
        public Int64 pctran1 { get; set; }
        public Int64 prcran1 { get; set; }
        public Int64 qtyiniran2 { get; set; }
        public Int64 pctran2 { get; set; }
        public Int64 prcran2 { get; set; }
        public Int64 indrecatriprc { get; set; }
        public Int64 indcalctriprc { get; set; }

    }

    public class PrecioEnLineaResponse
    {
        public Int64 num_eve_prc { get; set; }
        public List<string> errores { get; set; }

        public PrecioEnLineaResponse()
        {
            this.errores = new List<string>();
        }
    }


    public class BuscarPrecioEnLinea
    {
        public Int64 org_lvl_child { get; set; }
        public Int64 org_lvl_number { get; set; }
        public string org_name_full { get; set; } = string.Empty;
        public Int64 prd_lvl_child { get;set; }
        public string prd_lvl_number { get;set;} = string.Empty;
        public string prd_full_name { get; set; } = string.Empty;
        public decimal prc_vta { get; set; }
        public Int64 ind_evento { get; set; }
        public string usr_cre { get; set; }= string.Empty;
        public DateTime? fec_cre { get;set;}
        public Int64 est_proc { get; set; }
        public string des_est { get; set; }
        public DateTime? fec_proc { get; set; }
        public string ind_evento_txt { get; set; } = string.Empty;
        public string des_est_dad { get; set; } = string.Empty;
        public DateTime? proc_dad_date { get; set; }
    }


}
