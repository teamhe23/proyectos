using IntegracionBbook.Api.Models.Utils;
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
    public class StoreRepository : IStoreRepository
    {
        string ErrorMessage;
        private readonly IDBOracleRepository _iDBOracleRepository; //Interface de Conexion a Oracle
        private readonly ICommonRepository _commonRepository; //Interface de metodos comunes
        private List<PruebaInterna> pruebaInternas;
        public StoreRepository(IDBOracleRepository iDBOracleRepository, ICommonRepository commonRepository)
        {
            _iDBOracleRepository = iDBOracleRepository;
            _commonRepository = commonRepository;
        }
        #region Metodos Principales
        public DTO<Store>.Request GetAllStores(bool subproceso)
        {
            pruebaInternas = new List<PruebaInterna>();
            try
            {
                return new DTO<Store>.Request()
                {
                    data = GetStores("sp_store_read","-R"),
                    MessageError = pruebaInternas[0].message
                };
            }
            catch (Exception ex)
            {
                return new DTO<Store>.Request()
                {
                    MessageError = ex.Message
                };
            }
            finally
            {
                if (!subproceso) _iDBOracleRepository.Dispose();
            }
        }
        public async Task<DTO<Store>> LoadDataStore()
        {
            List<DTO<Store>.Request> storeRequests = new List<DTO<Store>.Request>();
            List<DTO<Store>.Response> storeResponses = new List<DTO<Store>.Response>();
            DTO<Store>.Request storeRequest = new DTO<Store>.Request();
            pruebaInternas = new List<PruebaInterna>();
            try
            {
                storeRequest = GetAllStores(true);
                if (storeRequest.data.Count == 0) storeRequest.data = GetStores("sp_store_load", "-L");
                else
                {
                    storeRequest.data = GetStores("sp_store_create", "-A");
                }
                if (storeRequest.data.Count != 0)
                {
                    //HACER PROCESO POST HACIA EL BBOOK
                    storeRequests.Add(new DTO<Store>.Request() { data = storeRequest.data });
                    storeResponses.Add(await GetResponseAndUpdateTable(storeRequest, "A"));
                }
                storeRequest.data = GetStores("sp_store_update","-C");
                if(storeRequest.data.Count != 0)
                {
                    //HACER PROCESO PUT HACIA EL BBOOK
                    storeRequests.Add(new DTO<Store>.Request() { data = storeRequest.data });
                    storeResponses.Add(await GetResponseAndUpdateTable(storeRequest, "C"));
                }                           
            }
            catch(Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    id = "01",
                    method = "LoadDataStore",
                    quantity = 0,
                    message = ex.Message,
                    table = false,
                    name = "Brand"
                });
            }
            finally
            {
                _commonRepository.Bbook_history("Bbook_Store", JsonConvert.SerializeObject(storeRequests),
                    JsonConvert.SerializeObject(storeResponses), JsonConvert.SerializeObject(pruebaInternas));
                _iDBOracleRepository.Dispose();
            }
            return new DTO<Store>()
            {
                requests = storeRequests,
                responses = storeResponses,
                pruebaInterna = new PruebaInternaDTO() { data = pruebaInternas }
            };
        }
        #endregion MetodosPrincipales
        #region Metodos Extras
        public List<Store> GetStores(string storedProcedure,string tipoproceso)
        {
            //Busca y carga sucursales nuevas a la tabla Book_Store //Consulta todas las sucursales en la tabla Book_Store
            List<Store> Stores = new List<Store>();
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;

            try
            {
                oracleCommand.Parameters.Add("Stores_C", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, storedProcedure, true))
                {
                    if (ErrorMessage == "OK")
                    {
                        while (oracleDataReader.Read())
                        {
                            Stores.Add(new Store()
                            {
                                id = oracleDataReader["store_id"].ToString().Trim(),
                                name = oracleDataReader["store_name"].ToString().Trim()
                            });
                        }
                    }
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetStores"+ tipoproceso,
                        quantity = Stores.Count,
                        message = ErrorMessage,
                        name = "Store",
                        table = false
                    });
                }
                else
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetStores"+ tipoproceso,
                        quantity = 0,
                        table = false,
                        message = ErrorMessage,
                        name = "Store"
                    });
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInternaDTO.PruebaInterna()
                {
                    id = "01",
                    method = "GetStores"+ tipoproceso,
                    quantity = 0,
                    message = ex.Message,
                    name = "Store",
                    table = false
                });
            }
            return Stores;
        }
        public async Task<DTO<Store>.Response> GetResponseAndUpdateTable(DTO<Store>.Request storeRequest, string tipo)
        {
            HttpResponseMessage res;
            DTO<Store>.Response storeResponse = new DTO<Store>.Response();
            List<string> codigos = new List<string>();
            ExceptionalErrorDTO<Store> exceptionalError;

            res = await _commonRepository.ApiBbook(JsonConvert.SerializeObject(storeRequest), tipo, "stores");
            if (res.StatusCode == HttpStatusCode.OK || res.StatusCode == HttpStatusCode.Conflict)
            {
                try
                {
                    storeResponse = JsonConvert.DeserializeObject<DTO<Store>.Response>(res.Content.ReadAsStringAsync().Result);
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        codigos = (from c in storeRequest.data select c.id).ToList();
                        foreach (var item in _commonRepository.UpdateDateTable("store", tipo, codigos))
                        {
                            pruebaInternas.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        exceptionalError = JsonConvert.DeserializeObject<ExceptionalErrorDTO<Store>>(res.Content.ReadAsStringAsync().Result);
                        return new DTO<Store>.Response()
                        {
                            internalCode = exceptionalError.internalCode,
                            message = exceptionalError.message,
                            statusCode = exceptionalError.statusCode,
                            status = exceptionalError.status,
                        };
                    }
                    catch
                    {
                        storeResponse = new DTO<Store>.Response()
                        {
                            internalCode = "JsonError",
                            message = "Error al deserializar Json" + "\n" + ex.Message,
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
                    storeResponse = new DTO<Store>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 401,
                        status = "error"
                    };
                }
                else if (res.StatusCode == HttpStatusCode.RequestEntityTooLarge)
                {
                    storeResponse = new DTO<Store>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 413,
                        status = "error"
                    };
                }
                else if (res.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    storeResponse = new DTO<Store>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 504,
                        status = "error"
                    };
                }
            }
            return storeResponse;
        }
        public PruebaInterna DeleteBbook_Store()
        {
            return _commonRepository.DeleteTable("Bbook_store");
        }
        #endregion Metodos Extras
    }
}
