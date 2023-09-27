using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Tienda
    {
        public int org_lvl_child { get; set; }
        public int codigo { get; set; }
        public string descripcion { get; set; }=string.Empty;

        public string org_is_store { get; set; } =string.Empty;
       
       
    }
}
