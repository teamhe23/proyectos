using IntegracionBbook.Models.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Models
{
    public class Master_po
    {
        [JsonIgnore]
        public string Master_po_id {get;set;}
        public string purchase_order { get; set; }
        public string vendor_id { get; set; }
        public string currency { get; set; }
        public string incoterm { get; set; }
        public string port_of_loading { get; set; }
        public string port_of_discharge { get; set; }
        public string payment_terms { get; set; }
        public string creation_date { get; set; }
        public List<SKU> details { get; set; }
        public class SKU
        {
            public string sku { get; set; }
            public int units { get; set; }
            public double unit_cost { get; set; }
        }
    }
}
