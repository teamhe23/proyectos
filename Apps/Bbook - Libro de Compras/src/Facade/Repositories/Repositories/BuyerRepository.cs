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
    public class BuyerRepository:IBuyerRepository
    {
        string ErrorMessage;
        private readonly IDBOracleRepository _iDBOracleRepository; //Interface de Conexion a Oracle
        private readonly ICommonRepository _commonRepository; //Interface de metodos comunes
        private List<PruebaInterna> pruebaInternas;
        public BuyerRepository(IDBOracleRepository iDBOracleRepository, ICommonRepository commonRepository)
        {
            _iDBOracleRepository = iDBOracleRepository;
            _commonRepository = commonRepository;
        }
        #region Metodos Principales
        public DTO<Buyer>.Request GetAllBuyers()
        {
            pruebaInternas = new List<PruebaInterna>();
            try
            {
                return new DTO<Buyer>.Request()
                {
                    data = GetBuyers("sp_buyer_read",""),
                    MessageError = pruebaInternas[0].message
                };
            }
            catch(Exception ex)
            {
                return new DTO<Buyer>.Request()
                {
                    MessageError = ex.Message
                };
            }
            finally
            {
                _iDBOracleRepository.Dispose();
            }
        }
        public async Task<DTO<Buyer>> LoadDataBuyer()
        {
            List<DTO<Buyer>.Request> buyerRequests = new List<DTO<Buyer>.Request>();
            List<DTO<Buyer>.Response> buyerResponses = new List<DTO<Buyer>.Response>();
            DTO<Buyer>.Request buyerRequest = new DTO<Buyer>.Request();
            pruebaInternas = new List<PruebaInterna>();

            try
            {
                buyerRequest.data = GetBuyers("sp_buyer_create","-A");
                if (buyerRequest.data.Count != 0)
                {
                    //HACER PROCESO POST HACIA EL BBOOK
                    buyerRequests.Add(new DTO<Buyer>.Request() { data = buyerRequest.data });
                    buyerResponses.Add(await GetResponseAndUpdateTable(buyerRequest, "A"));
                }
                buyerRequest.data = GetBuyers("sp_buyer_update","-C");
                if (buyerRequest.data.Count != 0)
                {
                    //HACER PROCESO PUT HACIA EL BBOOK
                    buyerRequests.Add(new DTO<Buyer>.Request() { data = buyerRequest.data });
                    buyerResponses.Add(await GetResponseAndUpdateTable(buyerRequest, "C"));
                }
                buyerRequest.data = GetBuyers("sp_buyer_update_inactivity", "-CI");
                if (buyerRequest.data.Count != 0)
                {
                    //HACER PROCESO PUT HACIA EL BBOOK
                    buyerRequests.Add(new DTO<Buyer>.Request() { data = buyerRequest.data });
                    buyerResponses.Add(await GetResponseAndUpdateTable(buyerRequest, "C"));
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    id = "01",
                    method = "LoadDataBuyer",
                    quantity = 0,
                    message = ex.Message,
                    name = "Buyer",
                    table = false
                });
            }
            finally
            {
                _commonRepository.Bbook_history("Bbook_Buyer", JsonConvert.SerializeObject(buyerRequests),
                    JsonConvert.SerializeObject(buyerResponses), JsonConvert.SerializeObject(pruebaInternas));
                _iDBOracleRepository.Dispose();
            }
            return new DTO<Buyer>()
            {
                requests = buyerRequests,
                responses = buyerResponses,
                pruebaInterna = new PruebaInternaDTO() { data = pruebaInternas }
            };
        }
        #endregion MetodosPrincipales
        #region Metodos Extras
        public List<Buyer> GetBuyers(string storedProcedure,string tipoproceso)
        {
            //Busca y carga sucursales nuevas a la tabla Book_Buyer //Consulta todas las sucursales en la tabla Book_Buyer
            List<Buyer> Buyers = new List<Buyer>();
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;
            bool inactive=false;

            try
            {
                oracleCommand.Parameters.Add("Buyers_C", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand,ref ErrorMessage, storedProcedure, true))
                {
                    if(ErrorMessage == "OK")
                    {
                        while (oracleDataReader.Read())
                        {
                            if (tipoproceso == "-CI") inactive = true;
                            Buyers.Add(new Buyer()
                            {
                                buyer_id = oracleDataReader["buyer_id"].ToString().Trim(),
                                buyer_name = oracleDataReader["buyer_name"].ToString().Trim(),
                                inactive = inactive
                            });
                        }
                    }
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetBuyers"+tipoproceso,
                        quantity = Buyers.Count,
                        message = ErrorMessage,
                        name = "Buyer",
                        table = false
                    });
                }
                else
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetBuyers" + tipoproceso,
                        quantity = 0,
                        message = ErrorMessage,
                        name = "Buyer",
                        table = false
                    });
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    id = "01",
                    method = "GetBuyers" + tipoproceso,
                    quantity = 0,
                    message = ex.Message,
                    table = false,
                    name = "Buyer"
                });
            }
            return Buyers;
        }
        public async Task<DTO<Buyer>.Response> GetResponseAndUpdateTable(DTO<Buyer>.Request buyerRequest, string tipo)
        {
            HttpResponseMessage res;
            DTO<Buyer>.Response buyerResponse = new DTO<Buyer>.Response();
            List<string> codigos = new List<string>();
            ExceptionalErrorDTO<Buyer> exceptionalError;

            res = await _commonRepository.ApiBbook(JsonConvert.SerializeObject(buyerRequest), tipo, "buyers");
            if (res.StatusCode == HttpStatusCode.OK || res.StatusCode == HttpStatusCode.Conflict)
            {
                try
                {
                    buyerResponse = JsonConvert.DeserializeObject<DTO<Buyer>.Response>(res.Content.ReadAsStringAsync().Result);
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        codigos = (from c in buyerRequest.data select c.buyer_id).ToList();
                        foreach (var item in _commonRepository.UpdateDateTable("buyer", tipo, codigos))
                        {
                            pruebaInternas.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        exceptionalError = JsonConvert.DeserializeObject<ExceptionalErrorDTO<Buyer>>(res.Content.ReadAsStringAsync().Result);
                        return new DTO<Buyer>.Response()
                        {
                            internalCode = exceptionalError.internalCode,
                            message = exceptionalError.message,
                            statusCode = exceptionalError.statusCode,
                            status = exceptionalError.status,
                        };
                    }
                    catch
                    {
                        buyerResponse = new DTO<Buyer>.Response()
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
                    buyerResponse = new DTO<Buyer>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 401,
                        status = "error"
                    };
                }
                else if (res.StatusCode == HttpStatusCode.RequestEntityTooLarge)
                {
                    buyerResponse = new DTO<Buyer>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 413,
                        status = "error"
                    };
                }
                else if (res.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    buyerResponse = new DTO<Buyer>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 504,
                        status = "error"
                    };
                }
            }
            return buyerResponse;
        }
        public PruebaInterna DeleteBbook_Buyer()
        {
            return _commonRepository.DeleteTable("Bbook_buyer");
        }
        #endregion Metodos Extras
    }
}
