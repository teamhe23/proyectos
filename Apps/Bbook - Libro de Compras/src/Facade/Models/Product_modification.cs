using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Api.Models
{
    public class Product_modification
    {
        public string parent_sku { get; set; }
        public string dimension_id { get; set; }
        public string description { get; set; }
        public string precio_blanco { get; set; }
        public List<Attribute> attributes { get; set; }
        public class Attribute
        {
            public string code { get; set; }
            public string name { get; set; }
            public string value { get; set; }
        }
    }
}
