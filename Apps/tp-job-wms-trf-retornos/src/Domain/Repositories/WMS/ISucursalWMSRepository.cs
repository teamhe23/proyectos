using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories.WMS
{
    public interface ISucursalWMSRepository 
    {
        Task<HttpResponseMessage> Post(string Model);
    }
}
