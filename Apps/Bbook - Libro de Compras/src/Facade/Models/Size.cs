using IntegracionBbook.Models.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Models
{
    public class Size
    {
        public string size_id { get; set; }
        public string type_id { get; set; }
        public string type_name { get; set; }
        public List<string> sizes { get; set; }
        public bool inactive => false;
    }
}
