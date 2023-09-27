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
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Repositories.Repositories
{
    public class DimensionRepository : IDimensionRepository
    {
        string ErrorMessage;
        private readonly IDBOracleRepository _iDBOracleRepository; //Interface de Conexion a Oracle
        private readonly ICommonRepository _commonRepository; //Interface de metodos comunes
        private List<PruebaInterna> pruebaInternas;
        public DimensionRepository(IDBOracleRepository iDBOracleRepository, ICommonRepository commonRepository)
        {
            _iDBOracleRepository = iDBOracleRepository;
            _commonRepository = commonRepository;
        }
        #region Metodos Principales
        public DTO<Dimension>.Request GetAllDimensions()
        {
            pruebaInternas = new List<PruebaInterna>();
            try
            {
                return new DTO<Dimension>.Request()
                {
                    data = GetDimensions("sp_Dimension_read",""),
                    MessageError = pruebaInternas[0].message
                };
            }
            catch (Exception ex)
            {
                return new DTO<Dimension>.Request()
                {
                    MessageError = ex.Message
                };
            }
            finally
            {
                _iDBOracleRepository.Dispose();
            }
        }
        public async Task<DTO<Dimension>> LoadDataDimension()
        {
            List<DTO<Dimension>.Request> dimensionRequests = new List<DTO<Dimension>.Request>();
            List<DTO<Dimension>.Response> dimensionResponses = new List<DTO<Dimension>.Response>();
            DTO<Dimension>.Request dimensionRequest = new DTO<Dimension>.Request();
            pruebaInternas = new List<PruebaInterna>();

            try
            {
                dimensionRequest.data = GetDimensions("sp_dimension_create","-A");
                if (dimensionRequest.data.Count != 0)
                {
                    //HACER PROCESO POST HACIA EL BBOOK
                    dimensionRequests.Add(new DTO<Dimension>.Request() { data = dimensionRequest.data });
                    dimensionResponses.Add(await GetResponseAndUpdateTable(dimensionRequest, "A"));
                }
                dimensionRequest.data = GetDimensions("sp_dimension_update","-C");
                if (dimensionRequest.data.Count != 0)
                {
                    //HACER PROCESO PUT HACIA EL BBOOK
                    dimensionRequests.Add(dimensionRequest);
                    dimensionResponses.Add(await GetResponseAndUpdateTable(dimensionRequest, "C"));
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    id = "01",
                    method = "LoadDataDimension",
                    quantity = 0,
                    message = ex.Message,
                    table = false,
                    name = "Dimension"
                });
            }
            finally
            {
                _commonRepository.Bbook_history("Bbook_Dimension", JsonConvert.SerializeObject(dimensionRequests),
                    JsonConvert.SerializeObject(dimensionResponses), JsonConvert.SerializeObject(pruebaInternas));
                _iDBOracleRepository.Dispose();
            }
            return new DTO<Dimension>()
            {
                requests = dimensionRequests,
                responses = dimensionResponses,
                pruebaInterna = new PruebaInternaDTO() { data = pruebaInternas}
            };
        }
        #endregion MetodosPrincipales
        #region Metodos Extras
        public List<Dimension> GetDimensions(string DimensiondProcedure,string tipoproceso)
        {
            //Busca y carga dimensiones nuevas a la tabla Bbook_Dimension //Consulta todas las dimensiones en la tabla Book_Dimension
            List<Dimension> Dimensions = new List<Dimension>();
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;

            try
            {
                oracleCommand.Parameters.Add("Dimensions_C", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand,ref ErrorMessage, DimensiondProcedure, true))
                {
                    if(ErrorMessage == "OK")
                    {
                        while (oracleDataReader.Read())
                        {
                            Dimensions.Add(new Dimension()
                            {
                                dimension_id = oracleDataReader["dimension_id"].ToString().Trim(),
                                dimension_name = oracleDataReader["dimension_name"].ToString().Trim(),
                                type_id = oracleDataReader["type_id"].ToString().Trim(),
                                type_name = oracleDataReader["type_name"].ToString().Trim()
                            });
                        }
                    }
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetDimensions"+tipoproceso,
                        quantity = Dimensions.Count,
                        message = ErrorMessage,
                        name = "Dimension",
                        table = false
                    });
                }
                else
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetDimensions"+tipoproceso,
                        quantity = 0,
                        message = ErrorMessage,
                        name = "Dimension",
                        table = false
                    });
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInternaDTO.PruebaInterna()
                {
                    id = "01",
                    method = "GetDimensions"+tipoproceso,
                    quantity = 0,
                    message = ex.Message,
                    name = "Dimension",
                    table = false
                });
            }
            return Dimensions;
        }
        public async Task<DTO<Dimension>.Response> GetResponseAndUpdateTable(DTO<Dimension>.Request dimensionRequest, string tipo)
        {
            HttpResponseMessage res;
            DTO<Dimension>.Response dimensionResponse = new DTO<Dimension>.Response();
            List<string> codigos = new List<string>();
            ExceptionalErrorDTO<Dimension> exceptionalError;

            res = await _commonRepository.ApiBbook(JsonConvert.SerializeObject(dimensionRequest), tipo, "dimensions");
            if (res.StatusCode == HttpStatusCode.OK || res.StatusCode == HttpStatusCode.Conflict)
            {
                try
                {
                    dimensionResponse = JsonConvert.DeserializeObject<DTO<Dimension>.Response>(res.Content.ReadAsStringAsync().Result);
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        codigos = (from c in dimensionRequest.data select c.dimension_id).ToList();
                        foreach (var item in _commonRepository.UpdateDateTable("dimension", tipo, codigos))
                        {
                            pruebaInternas.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        exceptionalError = JsonConvert.DeserializeObject<ExceptionalErrorDTO<Dimension>>(res.Content.ReadAsStringAsync().Result);
                        return new DTO<Dimension>.Response()
                        {
                            internalCode = exceptionalError.internalCode,
                            message = exceptionalError.message,
                            statusCode = exceptionalError.statusCode,
                            status = exceptionalError.status
                        };
                    }
                    catch
                    {
                        dimensionResponse = new DTO<Dimension>.Response()
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
                    dimensionResponse = new DTO<Dimension>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 401,
                        status = "error"
                    };
                }
                else if (res.StatusCode == HttpStatusCode.RequestEntityTooLarge)
                {
                    dimensionResponse = new DTO<Dimension>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 413,
                        status = "error"
                    };
                }
                else if (res.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    dimensionResponse = new DTO<Dimension>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 504,
                        status = "error"
                    };
                }
            }
            return dimensionResponse;
        }
        public PruebaInterna DeleteBbook_Dimension()
        {
            return _commonRepository.DeleteTable("Bbook_Dimension");
        }
        #endregion Metodos Extras
    }
}
