using IntegracionBbook.Models.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Models
{
    public class Buyer
    {
        public string buyer_id { get; set; }
        public string buyer_name { get; set; }
        public bool inactive { get; set; }
    }
}
