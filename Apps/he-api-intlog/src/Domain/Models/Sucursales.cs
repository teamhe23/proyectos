using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Sucursales
    {
        public decimal Id { get; set; }

        public decimal codigo { get; set; } 
        public string descripcion { get; set; } = string.Empty;

        public decimal padre { get; set; }
        public DateTime? fec_ape { get; set; }

        
    }
}
