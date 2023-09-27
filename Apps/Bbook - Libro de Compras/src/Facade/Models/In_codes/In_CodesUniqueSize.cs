using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IntegracionBbook.Models.In_Codes;

namespace IntegracionBbook.Api.Models.In_Codes
{
    public class In_CodesUniqueSize
    {
        public int master_code { get; set; }
        public bool is_unique_size { get; set; }
        public Generico line { get; set; }
        public string description { get; set; }
        public Brand brand { get; set; }
        public Generico vendor { get; set; }
        public List<GsAttr> gs_attrs { get; set; }
    }
}
