using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Models
{
    public class ExceptionalErrorDTO<T>: ResponseBbookDTO
    {
        public List<ErrorDTO<T>> errors { get; set; }
    }
}
