using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Models
{
    public class ResponseBbookDTO
    {
        public string status { get; set; }
        public int statusCode { get; set; }
        public string internalCode { get; set; }
        public string message { get; set; }
        public string bulkdata { get; set; }
    }
}
