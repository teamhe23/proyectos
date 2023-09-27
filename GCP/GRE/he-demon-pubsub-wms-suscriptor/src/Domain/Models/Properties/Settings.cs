using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Properties
{
    public class Settings
    {
        public int      TiempoEsperaSegundos    { get; set; }
        public string   RutaGCPCredenciales     { get; set; } = string.Empty;
    }
}
