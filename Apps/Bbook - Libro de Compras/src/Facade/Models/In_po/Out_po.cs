using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Api.Models.In_po
{
    public class Out_Po
    {
        public int id_document { get; set; }
        public string purchase_order { get; set; }
        public int total_units { get; set; }
        public double total_cost { get; set; }
        public List<Label> label { get; set; }
        public int status { get; set; }
        public string error { get; set; }

        public class Label
        {
            public string Tipo { get; set; }
            public string Numero { get; set; }
            public string Etiqueta { get; set; }
            public string EAN { get; set; }
            public string Desc1 { get; set; }
            public string Desc2 { get; set; }
            public string Dpto { get; set; }
            public string Linea { get; set; }
            public string Moneda { get; set; }
            public string Precio { get; set; }
            public string Sku { get; set; }
            public string Marca { get; set; }
            public string STQty { get; set; }
            public string Temporada { get; set; }
        }
    }
}
