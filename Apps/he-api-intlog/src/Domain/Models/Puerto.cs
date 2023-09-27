using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Puerto
    {
        public int id { get; set; }        
        public string descripcion { get; set; } = string.Empty;
        public int dias { get; set; } 
    }
}
