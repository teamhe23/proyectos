using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{    
    public class TipoIntegracion
    {
        public Int64 ID_TIPO { get; set; }
        public string TIPO_INTEGRACION { get; set; }
        public string HORA_INICIO { get; set; }
        public string HORA_FINAL { get; set; }
    }
}
