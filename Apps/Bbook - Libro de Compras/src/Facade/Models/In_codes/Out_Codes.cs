using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Api.Models.In_Codes
{
    public class Out_Codes
    {
        public int master_code { get; set; }
        public string parent_sku { get; set; }
        public List<DimValueOut> dim_values { get; set; }
        public string desc { get; set; }
        public string ean_upc { get; set; }
        public int status { get; set; }
        public string error { get; set; }
        public class DimValueOut
        {
            [JsonIgnore]
            public int master_code { get; set; }
            public string dimension_id { get; set; }
            public string style_prepack_code { get; set; }
            public List<SizeOut> sizes { get; set; }

            public class SizeOut
            {
                [JsonIgnore]
                public int master_code { get; set; }
                [JsonIgnore]
                public string dimCode { get; set; }
                public string size { get; set; }
                public string sku { get; set; }
                public string desc { get; set; }
                public string ean_upc { get; set; }
            }

        }
    }
}
