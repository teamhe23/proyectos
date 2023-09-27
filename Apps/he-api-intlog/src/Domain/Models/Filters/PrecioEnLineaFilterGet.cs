using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Filters
{
    public class PrecioEnLineaFilterGet
    {
        public Int64? sucursal { get; set; }
        public Int64? producto { get; set; }
        public  string fechainicio { get; set; } = string.Empty;
        public string fechafin { get; set; } = string.Empty;
    }
}
