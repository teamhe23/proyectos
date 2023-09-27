using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IntegracionBbook.Models.In_Codes;

namespace IntegracionBbook.Api.Models.In_Codes
{
    public class In_CodesNoUniqueSize
    {
        public int master_code { get; set; }
        public bool is_unique_size { get; set; }
        public string parent_sku { get; set; }
        public Generico line { get; set; }
        public string description { get; set; }
        public Generico size_type_id { get; set; }
        public Brand brand { get; set; }
        public List<string> sizes { get; set; }
        public Generico vendor { get; set; }
        public DimType dim_type { get; set; }
        public List<DimValue> dim_values { get; set; }

        public class DimType
        {
            public string code { get; set; }
            public string name { get; set; }
        }
    }
}
