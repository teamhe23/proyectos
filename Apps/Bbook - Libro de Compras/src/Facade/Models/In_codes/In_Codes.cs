using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models;
using IntegracionBbook.Models.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace IntegracionBbook.Models
{ 
    public class In_Codes 
    {
        public int master_code { get; set; }
        public bool is_unique_size { get; set; }
        public Generico line { get; set; }
        public string description { get; set; }
        public string packing_type { get; set; }
        public Brand brand { get; set; }
        public Generico size_type_id { get; set; }
        public Generico vendor { get; set; }
        public string parent_sku { get; set; }
        public List<string> sizes { get; set; }
        public DimType dim_type { get; set; }
        public List<DimValue> dim_values { get; set; }
        public List<GsAttr> gs_attrs { get; set; }

        public class DimType
        {
            public string code { get; set; }
            public string name { get; set; }
        }               
        public class GsAttr
        {
            public string code { get; set; }
            public string name { get; set; }
            public string value { get; set; }
        }
        public class DimValue
            {
                public string id { get; set; }
                public string name { get; set; }
                public List<int> ratio { get; set; }
                public List<GsAttr> gs_attrs { get; set; }
            }
    }
}
