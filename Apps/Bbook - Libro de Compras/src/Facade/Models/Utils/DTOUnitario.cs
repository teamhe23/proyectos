using IntegracionBbook.Models;
using IntegracionBbook.Models.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Api.Models.Utils
{
    public class DTOUnitario<T>
    {
        public Request request { get; set; }
        public Response response { get; set; }
        public PruebaInternaDTO pruebaInterna { get; set; }
        public class Request : RequestGeneral
        {
            [JsonProperty(PropertyName = "data")]
            public T data { get; set; }
        }
        public class Response : ResponseBbookDTO
        {
            public T data { get; set; }
            public List<ErrorDTO<T>> errors { get; set; }
        }
    }
}
