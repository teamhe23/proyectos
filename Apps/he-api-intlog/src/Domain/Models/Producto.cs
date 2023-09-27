using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Producto
    {        
        public int      prd_lvl_child       { get; set; }
        public string   prd_lvl_number      { get; set; } = string.Empty;
        public string   prd_name_full       { get; set; } = string.Empty;
        public decimal  costo_case_pack     { get; set; }
        public int      master_pack         { get; set; }
        public int      cantidad_minima     { get; set; }
    }

    public class ProductoPrecio
    {
        public Int64    prd_lvl_child       { get; set; }
        public string   prd_lvl_number      { get; set; } = string.Empty;
        public string   prd_full_name       { get; set; } = string.Empty;
        public string   cod_umv             { get; set; } = string.Empty;
        public bool     triprecio           { get; set; }
        public bool     preciazo            { get; set; }
        public decimal  precio_etiqueta     { get; set; }
        public decimal  costo_unitario      { get; set; }
    }
}
