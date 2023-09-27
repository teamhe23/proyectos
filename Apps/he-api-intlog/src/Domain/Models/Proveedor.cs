using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Proveedor
    {
        public int vpc_tech_key { get; set; }
        public string codigo { get; set; } = string.Empty;
        public string descripcion { get; set; } = string.Empty;
        public int pais { get; set; } 
        public string procedencia { get; set; } = string.Empty;
        public int dias_cancelacion { get; set; } 
        public string plazo { get; set; } = string.Empty;
        public int vpc_status_key { get; set; }
        public string moneda { get; set; } = string.Empty;

    }
}
