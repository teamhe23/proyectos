using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Api.Repositories.Utils;
using IntegracionBbook.Data.Interfaces;
using IntegracionBbook.Models;
using IntegracionBbook.Repositories.Interfaces;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Repositories.Repositories
{
    public class Received_productRepository: IReceived_productRepository
    {
        string ErrorMessage;
        private readonly IDBOracleRepository _iDBOracleRepository; //Interface de Conexion a Oracle
        private readonly ICommonRepository _commonRepository; //Interface de metodos comunes
        private List<PruebaInterna> pruebaInternas;
        public Received_productRepository(IDBOracleRepository iDBOracleRepository, ICommonRepository commonRepository)
        {
            _iDBOracleRepository = iDBOracleRepository;
            _commonRepository = commonRepository;
        }
        #region Metodos Principales
        public DTO<Received_product>.Request GetAllReceived_products()
        {
            pruebaInternas = new List<PruebaInterna>();
            try
            {
                return new DTO<Received_product>.Request()
                {
                    data = GetReceived_products("sp_received_product_read",""),
                    MessageError = pruebaInternas[0].message
                };
            }
            catch (Exception ex)
            {
                return new DTO<Received_product>.Request()
                {
                    MessageError = ex.Message
                };
            }
            finally
            {
                _iDBOracleRepository.Dispose();
            }
        }
        public async Task<DTO<Received_product>> LoadDataReceived_product()
        {
            List<DTO<Received_product>.Request> received_productRequests = new List<DTO<Received_product>.Request>();
            List<DTO<Received_product>.Response> received_productResponses = new List<DTO<Received_product>.Response>();
            DTO<Received_product>.Request received_productRequest = new DTO<Received_product>.Request();
            IEnumerable<IEnumerable<Received_product>> received_ProductRequestTemporal;
            pruebaInternas = new List<PruebaInterna>();
            try
            {
                received_productRequest.data = GetReceived_products("sp_received_product_create","-A");
                if (received_productRequest.data.Count != 0)
                {
                    //HACER PROCESO POST HACIA EL BBOOK
                    received_productRequests.Add(new DTO<Received_product>.Request() { data = received_productRequest.data });
                    if (received_productRequest.data.Count >= 2000)
                    {
                        received_ProductRequestTemporal = LinqExtensions.Split(received_productRequest.data, received_productRequest.data.Count / 2000);
                        foreach (var item in received_ProductRequestTemporal)
                        {
                            received_productRequest.data = (List<Received_product>)item.ToList();
                            received_productResponses.Add(await GetResponseAndUpdateTable(received_productRequest, "A"));
                        }
                    }
                    else received_productResponses.Add(await GetResponseAndUpdateTable(received_productRequest, "A"));
                }
                //received_productRequest.data = GetReceived_products("sp_received_product_update","-C");
                //if (received_productRequest.data.Count != 0)
                //{
                //    //HACER PROCESO PUT HACIA EL BBOOK
                //    received_productRequests.Add(new DTO<Received_product>.Request() { data = received_productRequest.data });
                //    received_productResponses.Add(await GetResponseAndUpdateTable(received_productRequest, "C"));
                //}
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    id = "01",
                    method = "LoadDataReceived_product",
                    quantity = 0,
                    message = ex.Message,
                    table = false,
                    name = "Received_product"
                });
            }
            finally
            {
                _commonRepository.Bbook_history("Bbook_Received_product", JsonConvert.SerializeObject(received_productRequests),
                    JsonConvert.SerializeObject(received_productResponses), JsonConvert.SerializeObject(pruebaInternas));
                _iDBOracleRepository.Dispose();
            }
            return new DTO<Received_product>()
            {
                requests = received_productRequests,
                responses = received_productResponses,
                pruebaInterna = new PruebaInternaDTO() { data = pruebaInternas }
            };
        }
        #endregion
        #region Metodos Extras
        public List<Received_product> GetReceived_products(string storedProcedure,string tipoproceso)
        {
            //Busca y carga marcas nuevas a la tabla Book_Received_product //Consulta todas las marcas en la tabla Book_Received_product
            List<Received_product> Received_products = new List<Received_product>();
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;
            int quantity;

            try
            {
                oracleCommand.Parameters.Add("Received_products_C", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand,ref ErrorMessage, storedProcedure, true))
                {
                    if(ErrorMessage == "OK")
                    {
                        while (oracleDataReader.Read())
                        {
                            quantity = oracleDataReader["quantity"].ToString().Trim() == "" ? 0 : int.Parse(oracleDataReader["quantity"].ToString().Trim());
                            Received_products.Add(new Received_product()
                            {
                                received_product_id = oracleDataReader["received_product_id"].ToString().Trim(),
                                warehouse_id = oracleDataReader["warehouse_id"].ToString().Trim(),
                                po_number = oracleDataReader["po_number"].ToString().Trim(),
                                receiving_date = _commonRepository.CambiarFormatoFecha(oracleDataReader["receiving_date"].ToString().Trim()),
                                sku = oracleDataReader["sku"].ToString().Trim(),
                                quantity = quantity
                            });
                        }
                    }
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetReceived_products"+ tipoproceso,
                        quantity = Received_products.Count,
                        message = ErrorMessage,
                        name = "Received_product",
                        table = false
                    });
                }
                else
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetReceived_products"+ tipoproceso,
                        quantity = 0,
                        message = ErrorMessage,
                        name = "Received_product",
                        table = false
                    });
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInternaDTO.PruebaInterna()
                {
                    id = "01",
                    method = "GetReceived_products"+ tipoproceso,
                    quantity = 0,
                    message = ex.Message,
                    name = "Received_product",
                    table = false
                });
            }
            return Received_products;
        }
        public async Task<DTO<Received_product>.Response> GetResponseAndUpdateTable(DTO<Received_product>.Request received_productRequest, string tipo)
        {
            HttpResponseMessage res = new HttpResponseMessage();
            DTO<Received_product>.Response received_productResponse = new DTO<Received_product>.Response();
            List<string> codigos = new List<string>();
            ExceptionalErrorDTO<Received_product> exceptionalError;

            res = await _commonRepository.ApiBbook(JsonConvert.SerializeObject(received_productRequest), tipo, "receiving-products");
            if (res.StatusCode == HttpStatusCode.OK || res.StatusCode == HttpStatusCode.Conflict)
            {
                try
                {
                    received_productResponse = JsonConvert.DeserializeObject<DTO<Received_product>.Response>(res.Content.ReadAsStringAsync().Result);
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        codigos = (from c in received_productRequest.data select c.received_product_id).ToList();
                        foreach (var item in _commonRepository.UpdateDateTable("received_product", tipo, codigos))
                        {
                            pruebaInternas.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        exceptionalError = JsonConvert.DeserializeObject<ExceptionalErrorDTO<Received_product>>(res.Content.ReadAsStringAsync().Result);
                        return new DTO<Received_product>.Response()
                        {
                            internalCode = exceptionalError.internalCode,
                            message = exceptionalError.message,
                            statusCode = exceptionalError.statusCode,
                            status = exceptionalError.status
                        };
                    }
                    catch
                    {
                        received_productResponse = new DTO<Received_product>.Response()
                        {
                            internalCode = "JsonError",
                            message = JsonConvert.SerializeObject(res.Content.ReadAsStringAsync().Result) +Environment.NewLine+ ex.Message,
                            statusCode = 00,
                            status = "error"
                        };
                    }
                }
            }
            else
            {
                if (res.StatusCode == HttpStatusCode.Unauthorized)
                {
                    received_productResponse = new DTO<Received_product>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 401,
                        status = "error"
                    };
                }
                else if (res.StatusCode == HttpStatusCode.RequestEntityTooLarge)
                {
                    received_productResponse = new DTO<Received_product>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 413,
                        status = "error"
                    };
                }
                else if (res.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    received_productResponse = new DTO<Received_product>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 504,
                        status = "error"
                    };
                }
            }
            return received_productResponse;
        }
        public PruebaInterna DeleteBbook_Received_product()
        {
            return _commonRepository.DeleteTable("Bbook_received_product");
        }
        #endregion
    }
}
