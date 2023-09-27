using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Models
{
    public class ErrorDTO<T>
    {
        public string code { get; set; }
        public string message { get; set; }
        public T record { get; set; }
    }
}
