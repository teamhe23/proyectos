using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{    
    public class Logs
    {
        public Int64 ID_TIPO { get; set; }
        public int IDENTIFICADOR { get; set; }
        public string MENSAJE { get; set; }
        public string TRAMA { get; set; }
    }
}
