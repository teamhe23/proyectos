using Domain.Models.Properties;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Domain.Models;
using System.Xml;
using Domain.Repositories.WMS;
using System.Net;

namespace Data.WMS.Repositories
{
    public class SucursalWMSRepository : ISucursalWMSRepository
    {
        private readonly ApiWMSProperties _apiWMSProperties;
        private readonly HttpClient _clientApi = new HttpClient();

        public SucursalWMSRepository(IOptions<ApiWMSProperties> apiWMSProperties)
        {
            _apiWMSProperties = apiWMSProperties.Value;

            _clientApi = new HttpClient();
            _clientApi.BaseAddress = new Uri(_apiWMSProperties.Url);
            _clientApi.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        private void AgregarBasicAutenticacion(HttpClient httpClient)
        {
            var key = Convert.ToBase64String(Encoding.ASCII.GetBytes(_apiWMSProperties.User + ":" + _apiWMSProperties.Password));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", key);
        }
        
        public async Task<HttpResponseMessage> Post(string Model)
        {
            HttpResponseMessage responseApi = new HttpResponseMessage();
            try
            {
                AgregarBasicAutenticacion(_clientApi);
                String suc = Model.Substring(4, 3).ToString();
                HttpContent content = new FormUrlEncodedContent(
                            new List<KeyValuePair<string, string>> {
                                new KeyValuePair<string, string>("entity","Order"),
                                new KeyValuePair<string, string>("async","1"),
                                new KeyValuePair<string, string>("flat_data",Model),
                                new KeyValuePair<string, string>("facility_code",suc)
                            }
                );
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                content.Headers.ContentType.CharSet = "UTF-8";
                responseApi = await _clientApi.PostAsync("wms/api/init_stage_interface/", content);
                return responseApi;

            }
            catch (Exception)
            {
                responseApi.StatusCode = HttpStatusCode.BadRequest;
                return responseApi;
            }
        }
    }
}
