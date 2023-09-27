using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntegracionBbook.Models.Utils;
using Newtonsoft.Json;

namespace IntegracionBbook.Models
{
    public class Store
        {
            public string id { get; set; }
            public string name { get; set; }
            public bool inactive => false;
        }
}
