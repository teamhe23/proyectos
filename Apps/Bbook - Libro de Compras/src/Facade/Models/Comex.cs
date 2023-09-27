using IntegracionBbook.Models.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Models
{
    public class Comex
        {
            [JsonIgnore]
            public string comex_id { get; set; }
            public string purchase_order { get; set; }

            [JsonProperty("comex_id")]
            public string comexid { get; set; }
            public string sku { get; set; }
            public int sent_quantity { get; set; }
            public string forwarder_date { get; set; }
            public string shipping_date { get; set; }
            public string port_arrival_date { get; set; }
        }
}
