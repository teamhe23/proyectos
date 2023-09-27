using IntegracionBbook.Models.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Models
{
    public class Received_product
    {
        public string received_product_id { get; set; }
        public string warehouse_id { get; set; }
        public string po_number { get; set; }
        public string receiving_date { get; set; }
        public string sku { get; set; }
        public int quantity { get; set; }
    }
}
