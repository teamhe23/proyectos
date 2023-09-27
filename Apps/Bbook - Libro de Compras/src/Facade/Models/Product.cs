using IntegracionBbook.Models.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Models
{
    public class Product
    {
        public string product_id { get; set; }
        public string brand_id { get; set; }
        public string hierarchy_id { get; set; }
        public bool is_unique_size { get; set; }
        public string dimension_id { get; set; }
        public string vendor_id { get; set; }
        public string dimension_type { get; set; }
        public string season_id { get; set; }
        public double master_price { get; set; }
        public string size_type_id { get; set; }
        public string style_desc { get; set; }
        public string parent_sku { get; set; }
        public StylePrePack style_pre_pack { get; set; }
        public List<Product_Size> sizes { get; set; }
        public class StylePrePack
        {
            public string code { get; set; }
            public List<string> curve { get; set; }
        }
        public class Product_Size
        {
            public string product_key { get; set; }
            public string size { get; set; }
            public string sku { get; set; }
            public string ean_upc { get; set; }
            public string description { get; set; }
        }
    }
}
