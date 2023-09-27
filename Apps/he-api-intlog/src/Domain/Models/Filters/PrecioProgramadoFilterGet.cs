using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Filters
{
    public class PrecioProgramadoFilterGet
    {
        public Int64?   org_lvl_child       { get; set; }
        public Int64?   prd_lvl_child       { get; set; }
        [Required]
        public string   fecha_ini           { get; set; } = string.Empty;
        [Required]
        public string   fecha_fin           { get; set; } = string.Empty;
    }
}
