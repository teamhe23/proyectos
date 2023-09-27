using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobs.Helpers
{
    public static class ProcesarTexto
    {
        public static string RecortarCadena(this string cadena, int longitud)
        {
            if (!string.IsNullOrEmpty(cadena))
            {
                cadena = cadena.Substring(0, cadena.Length > longitud ? longitud : cadena.Length);
            }

            return cadena;
        }
    }
}
