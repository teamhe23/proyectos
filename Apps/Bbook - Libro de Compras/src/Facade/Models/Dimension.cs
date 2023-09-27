using IntegracionBbook.Models.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Models
{
    public class Dimension
    {
        public string type_id { get; set; }
        public string type_name { get; set; }
        public string dimension_id { get; set; }
        public string dimension_name { get; set; }
        public bool inactive => false;
    }
}
