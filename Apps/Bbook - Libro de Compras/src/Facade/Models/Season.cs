using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models.Utils;
using Newtonsoft.Json;

namespace IntegracionBbook.Models
{
    public class Season:Generico
    {
        public bool inactive => false;
    }
}
