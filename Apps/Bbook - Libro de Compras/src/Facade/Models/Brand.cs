using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Models
{
    public class Brand:Generico
    {
        public bool inactive => false;
    }
}
