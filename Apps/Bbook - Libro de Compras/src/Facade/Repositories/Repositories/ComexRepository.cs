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
    public class ComexRepository:IComexRepository
    {
        string ErrorMessage;
        private readonly IDBOracleRepository _iDBOracleRepository; //Interface de Conexion a Oracle
        private readonly ICommonRepository _commonRepository; //Interface de metodos comunes
        private List<PruebaInterna> pruebaInternas;
        public ComexRepository(IDBOracleRepository iDBOracleRepository, ICommonRepository commonRepository)
        {
            _iDBOracleRepository = iDBOracleRepository;
            _commonRepository = commonRepository;
        }
        #region Metodos Principales
        public DTO<Comex>.Request GetAllComexs()
        {
            pruebaInternas = new List<PruebaInterna>();
            try
            {
                return new DTO<Comex>.Request()
                {
                    data = GetComexs("sp_comex_read",""),
                    MessageError = pruebaInternas[0].message
                };
            }
            catch (Exception ex)
            {
                return new DTO<Comex>.Request()
                {
                    MessageError = ex.Message
                };
            }
            finally
            {
                _iDBOracleRepository.Dispose();
            }
        }
        public async Task<DTO<Comex>> LoadDataComex()
        {
            List<DTO<Comex>.Request> comexRequests = new List<DTO<Comex>.Request>();
            List<DTO<Comex>.Response> comexResponses = new List<DTO<Comex>.Response>();
            IEnumerable<IEnumerable<Comex>> comexRequestTemporal;
            DTO<Comex>.Request comexRequest = new DTO<Comex>.Request();
            pruebaInternas = new List<PruebaInterna>();
            try
            {
                comexRequest.data = GetComexs("sp_comex_create","-A");
                if (comexRequest.data.Count != 0)
                {
                    //HACER PROCESO POST HACIA EL BBOOK
                    comexRequests.Add(new DTO<Comex>.Request() { data = comexRequest.data });
                    if(comexRequest.data.Count >= 2000)
                    {
                        comexRequestTemporal = LinqExtensions.Split(comexRequest.data, comexRequest.data.Count/2000);
                        foreach (var item in comexRequestTemporal)
                        {
                            comexRequest.data = (List<Comex>)item.ToList();
                            comexResponses.Add(await GetResponseAndUpdateTable(comexRequest, "A"));
                        }
                    }
                    else comexResponses.Add(await GetResponseAndUpdateTable(comexRequest, "A"));
                }
                //comexRequest.data = GetComexs("sp_comex_update","-C");
                //if (comexRequest.data.Count != 0)
                //{
                //    //HACER PROCESO PUT HACIA EL BBOOK
                //    comexRequests.Add(new DTO<Comex>.Request() { data = comexRequest.data });
                //    comexResponses.Add(await GetResponseAndUpdateTable(comexRequest, "C"));
                //}
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    id = "01",
                    method = "LoadDataComex",
                    quantity = 0,
                    message = ex.Message,
                    name = "Comex",
                    table = false
                });
            }
            finally
            {
                _commonRepository.Bbook_history("Bbook_Comex", JsonConvert.SerializeObject(comexRequests),
                   JsonConvert.SerializeObject(comexResponses), JsonConvert.SerializeObject(pruebaInternas));
                _iDBOracleRepository.Dispose();
            }
            return new DTO<Comex>()
            {
                requests = comexRequests,
                responses = comexResponses,
                pruebaInterna = new PruebaInternaDTO() { data = pruebaInternas }
            };
        }
        #endregion
        #region Metodos Extras
        public List<Comex> GetComexs(string storedProcedure,string tipoproceso)
        {
            //Busca y carga marcas nuevas a la tabla Book_Comex //Consulta todas las marcas en la tabla Book_Comex
            List<Comex> Comexs = new List<Comex>();
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;
            int cantidad;

            try
            {
                oracleCommand.Parameters.Add("Comexs_C", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand,ref ErrorMessage, storedProcedure, true))
                {
                    if(ErrorMessage == "OK")
                    {
                        while (oracleDataReader.Read())
                    {
                        int.TryParse(oracleDataReader["sent_quantity"].ToString(), out cantidad);
                        Comexs.Add(new Comex()
                        {
                            comex_id = oracleDataReader["comex_id"].ToString().Trim(),
                            purchase_order = oracleDataReader["purchase_order"].ToString().Trim(),
                            comexid = oracleDataReader["comexid"].ToString().Trim(),
                            sku = oracleDataReader["sku"].ToString().Trim(),
                            sent_quantity = cantidad,
                            forwarder_date = oracleDataReader["forwarder_date"].ToString().Trim() != ""
                             ? _commonRepository.CambiarFormatoFecha(oracleDataReader["forwarder_date"].ToString().Trim()):"",
                            shipping_date = oracleDataReader["shipping_date"].ToString().Trim() != ""
                            ? _commonRepository.CambiarFormatoFecha(oracleDataReader["shipping_date"].ToString().Trim()):"",
                            port_arrival_date = oracleDataReader["port_arrival_date"].ToString().Trim() != ""
                            ? _commonRepository.CambiarFormatoFecha(oracleDataReader["port_arrival_date"].ToString().Trim()):""
                        });
                    }
                    }
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetComexs"+ tipoproceso,
                        quantity = Comexs.Count,
                        message = ErrorMessage,
                        name = "Comex",
                        table = false
                    });
                }
                else
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetComexs"+ tipoproceso,
                        quantity = 0,
                        message = ErrorMessage,
                        name = "Comex",
                        table = false
                    });
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    id = "01",
                    method = "GetComexs" + tipoproceso,
                    quantity = 0,
                    message = ex.Message,
                    table = false,
                    name = "Comex"
                });
            }
            return Comexs;
        }
        public async Task<DTO<Comex>.Response> GetResponseAndUpdateTable(DTO<Comex>.Request comexRequest, string tipo)
        {
            HttpResponseMessage res = new HttpResponseMessage();
            DTO<Comex>.Response comexResponse = new DTO<Comex>.Response();
            List<string> codigos = new List<string>();

            ExceptionalErrorDTO<Comex> exceptionalError;

            res = await _commonRepository.ApiBbook(JsonConvert.SerializeObject(comexRequest), tipo, "comex");
            if (res.StatusCode == HttpStatusCode.OK || res.StatusCode == HttpStatusCode.Conflict)
            {
                try
                {
                    comexResponse = JsonConvert.DeserializeObject<DTO<Comex>.Response>(res.Content.ReadAsStringAsync().Result);
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        codigos = (from c in comexRequest.data select c.comex_id).ToList();
                        foreach (var item in _commonRepository.UpdateDateTable("comex", tipo, codigos))
                        {
                            pruebaInternas.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        exceptionalError = JsonConvert.DeserializeObject<ExceptionalErrorDTO<Comex>>(res.Content.ReadAsStringAsync().Result);
                        return new DTO<Comex>.Response()
                        {
                            internalCode = exceptionalError.internalCode,
                            message = exceptionalError.message,
                            statusCode = exceptionalError.statusCode,
                            status = exceptionalError.status
                        };
                    }
                    catch
                    {
                        comexResponse = new DTO<Comex>.Response()
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
                    comexResponse = new DTO<Comex>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 401,
                        status = "error"
                    };
                }
                else if (res.StatusCode == HttpStatusCode.RequestEntityTooLarge)
                {
                    comexResponse = new DTO<Comex>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 413,
                        status = "error"
                    };
                }
                else if (res.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    comexResponse = new DTO<Comex>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 504,
                        status = "error"
                    };
                }

            }
            return comexResponse;
        }
        public PruebaInterna DeleteBbook_Comex()
        {
            return _commonRepository.DeleteTable("Bbook_comex");
        }
        #endregion
    }
}
