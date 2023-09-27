using IntegracionBbook.Models.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
namespace Facade.Models.In_Comex
{
    public class In_Comex
    {

        public String nombre { get; set; }
        public List<Detalle_Comex> detalles { get; set; }
        public class Detalle_Comex 
        {
            public String oc { get; set; }
            public String item { get; set; }
            public String cod_producto { get; set; }
            public String estilo { get; set; }
            public String cantidad { get; set; }
            public String cant_factura { get; set; }
            public String fch_delivery_day { get; set; }
            public String fch_entrega_real { get; set; }
            public String fch_salida { get; set; }
            public String fch_llegada { get; set; }
        }
    }

}
