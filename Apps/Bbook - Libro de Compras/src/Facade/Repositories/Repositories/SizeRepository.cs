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
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static IntegracionBbook.Models.PruebaInternaDTO;
using IntegracionBbook.Api.Models.Utils;

namespace IntegracionBbook.Repositories.Repositories
{
    public class SizeRepository:ISizeRepository
    {
        string ErrorMessage;
        private readonly IDBOracleRepository _iDBOracleRepository; //Interface de Conexion a Oracle
        private readonly ICommonRepository _commonRepository; //Interface de metodos comunes
        private List<PruebaInterna> pruebaInternas;
        public class SizeTemporal
        {
            public string type_id { get; set; }
            public string size_type_name { get; set; }
        }
        public SizeRepository(IDBOracleRepository iDBOracleRepository, ICommonRepository commonRepository)
        {
            _iDBOracleRepository = iDBOracleRepository;
            _commonRepository = commonRepository;
        }

        #region Metodos Principales

        public DTO<Size>.Request GetAllSizes()
        {
            pruebaInternas = new List<PruebaInterna>();
            try
            {
                return new DTO<Size>.Request()
                {
                    data = GetSizes("sp_Size_read",""),
                    MessageError = pruebaInternas[0].message
                };
            }
            catch (Exception ex)
            {
                return new DTO<Size>.Request()
                {
                    MessageError = ex.Message
                };
            }
            finally
            {
                _iDBOracleRepository.Dispose();
            }
        }
        public async Task<DTO<Size>> LoadDataSize()
        {
            List<DTO<Size>.Request> sizeRequests = new List<DTO<Size>.Request>();
            List<DTO<Size>.Response> sizeResponses = new List<DTO<Size>.Response>();
            DTO<Size>.Request sizeRequest = new DTO<Size>.Request();
            pruebaInternas = new List<PruebaInterna>();

            try
            {
                sizeRequest.data = GetSizes("sp_Size_create","-A");
                if (sizeRequest.data.Count != 0)
                {
                    //HACER PROCESO POST HACIA EL BBOOK
                    sizeRequests.Add(new DTO<Size>.Request() { data = sizeRequest.data });
                    sizeResponses.Add(await GetResponseAndUpdateTable(sizeRequest, "A"));
                }
                sizeRequest.data = GetSizes("sp_Size_update","-C");
                if (sizeRequest.data.Count != 0)
                {
                    //HACER PROCESO PUT HACIA EL BBOOK
                    sizeRequests.Add(sizeRequest);
                    sizeResponses.Add(await GetResponseAndUpdateTable(sizeRequest, "C"));
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    id = "01",
                    method = "LoadDataSize",
                    quantity = 0,
                    message = ex.Message,
                    table = false,
                    name = "Size"
                });
            }
            finally
            {
                _commonRepository.Bbook_history("Bbook_Size", JsonConvert.SerializeObject(sizeRequests),
                    JsonConvert.SerializeObject(sizeResponses), JsonConvert.SerializeObject(pruebaInternas));
                _iDBOracleRepository.Dispose();
            }
            return new DTO<Size>()
            {
                requests = sizeRequests,
                responses = sizeResponses,
                pruebaInterna = new PruebaInternaDTO() { data = pruebaInternas }
            };
        }

        #endregion MetodosPrincipales

        #region Metodos Extras

        public List<Size> GetSizes(string SizedProcedure,string tipoproceso)
        {
            //Busca y carga sucursales nuevas a la tabla Book_Size //Consulta todas las sucursales en la tabla Book_Size
            List<Size> Sizes = new List<Size>();
            List<SizeTemporal> sizes = new List<SizeTemporal>();
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;

            try
            {
                oracleCommand.Parameters.Add("Sizes_C", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand,ref ErrorMessage, SizedProcedure, true))
                {
                    if(ErrorMessage == "OK")
                    {
                        while (oracleDataReader.Read())
                        {
                            Sizes.Add(new Size()
                            {
                                size_id = oracleDataReader["size_id"].ToString().Trim(),
                                type_id = oracleDataReader["type_id"].ToString().Trim(),
                                type_name = oracleDataReader["type_name"].ToString().Trim()
                            });
                            sizes.Add(new SizeTemporal()
                            {
                                type_id = oracleDataReader["type_id"].ToString().Trim(),
                                size_type_name = oracleDataReader["size_type_name"].ToString().Trim()
                            });
                        }
                        Sizes = Sizes.GroupBy(x => x.type_id)
                         .Select(group => group.First()).ToList();
                        for (int i = 0; i < Sizes.Count; i++)
                        {
                            Sizes[i].sizes = (from x in sizes where x.type_id == Sizes[i].type_id select x.size_type_name).ToList();
                        }
                    }
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetSizes"+ tipoproceso,
                        quantity = Sizes.Count,
                        message = ErrorMessage,
                        name = "Size",
                        table = false
                    });
                }
                else
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetSizes"+ tipoproceso,
                        message = ErrorMessage,
                        quantity = 0,
                        name = "Size",
                        table = false
                    });
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInternaDTO.PruebaInterna()
                {
                    id = "01",
                    method = "GetSizes"+ tipoproceso,
                    quantity = 0,
                    message = ex.Message,
                    name = "Size",
                    table = false
                });
            }
            return Sizes;
        }

        public async Task<DTO<Size>.Response> GetResponseAndUpdateTable(DTO<Size>.Request sizeRequest, string tipo)
        {
            HttpResponseMessage res;
            DTO<Size>.Response sizeResponse = new DTO<Size>.Response();
            List<string> codigos = new List<string>();
            ExceptionalErrorDTO<Size> exceptionalError;

            res = await _commonRepository.ApiBbook(JsonConvert.SerializeObject(sizeRequest), tipo, "sizes");
            if (res.StatusCode == HttpStatusCode.OK || res.StatusCode == HttpStatusCode.Conflict)
            {
                try
                {
                    sizeResponse = JsonConvert.DeserializeObject<DTO<Size>.Response>(res.Content.ReadAsStringAsync().Result);
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        codigos = (from c in sizeRequest.data select c.type_id).ToList();
                        foreach (var item in _commonRepository.UpdateDateTableSize("size", tipo, codigos))
                        {
                            pruebaInternas.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        exceptionalError = JsonConvert.DeserializeObject<ExceptionalErrorDTO<Size>>(res.Content.ReadAsStringAsync().Result);
                        return new DTO<Size>.Response()
                        {
                            internalCode = exceptionalError.internalCode,
                            message = exceptionalError.message,
                            statusCode = exceptionalError.statusCode,
                            status = exceptionalError.status
                        };
                    }
                    catch
                    {
                        sizeResponse = new DTO<Size>.Response()
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
                    sizeResponse = new DTO<Size>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 401,
                        status = "error"
                    };
                }
                else if (res.StatusCode == HttpStatusCode.RequestEntityTooLarge)
                {
                    sizeResponse = new DTO<Size>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 413,
                        status = "error"
                    };
                }
                else if (res.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    sizeResponse = new DTO<Size>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 504,
                        status = "error"
                    };
                }
            }
            return sizeResponse;
        }

        public PruebaInterna DeleteBbook_Size()
        {
            return _commonRepository.DeleteTable("Bbook_Size");
        }

        #endregion Metodos Extras
    }
}
