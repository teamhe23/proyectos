using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Models
{
    public class PruebaInternaDTO
    {
        public List<PruebaInterna> data { get; set; }
        public class PruebaInterna
        {
            public bool table { get; set; }
            public string name { get; set; }
            public string method { get; set; }
            public string message { get; set; }
            public string id { get; set; }
            public int quantity { get; set; }
        }
    }
}
