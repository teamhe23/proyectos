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
    public class SeasonRepository:ISeasonRepository
    {
        string ErrorMessage;
        private readonly ICommonRepository _commonRepository; //Interface de metodos comunes
        private readonly IDBOracleRepository _iDBOracleRepository; //Interface de Conexion a Oracle
        private List<PruebaInterna> pruebaInternas;
        public SeasonRepository(IDBOracleRepository iDBOracleRepository,ICommonRepository commonRepository)
        {
            _iDBOracleRepository = iDBOracleRepository;
            _commonRepository = commonRepository;
        }
        #region Metodos Principales
        public DTO<Season>.Request GetAllSeasons()
        {
            pruebaInternas = new List<PruebaInterna>();
            try
            {
                return new DTO<Season>.Request()
                {
                    data = GetSeasons("sp_Season_read",""),
                    MessageError = pruebaInternas[0].message
                };
            }
            catch (Exception ex)
            {
                return new DTO<Season>.Request()
                {
                    MessageError = ex.Message
                };
            }
            finally
            {
                _iDBOracleRepository.Dispose();
            }
        }
        public async Task<DTO<Season>> LoadDataSeason()
        {
            List<DTO<Season>.Request> seasonRequests = new List<DTO<Season>.Request>();
            List<DTO<Season>.Response> seasonResponses = new List<DTO<Season>.Response>();
            DTO<Season>.Request seasonRequest = new DTO<Season>.Request();
            pruebaInternas = new List<PruebaInterna>();
            try
            {
                seasonRequest.data = GetSeasons("sp_season_create","-A");
                if (seasonRequest.data.Count != 0)
                {
                    //HACER PROCESO POST HACIA EL BBOOK
                    seasonRequests.Add(new DTO<Season>.Request() { data = seasonRequest.data });
                    seasonResponses.Add(await GetResponseAndUpdateTable(seasonRequest, "A"));
                }
                seasonRequest.data = GetSeasons("sp_season_update","-C");
                if (seasonRequest.data.Count != 0)
                {
                    //HACER PROCESO PUT HACIA EL BBOOK
                    seasonRequests.Add(new DTO<Season>.Request() { data = seasonRequest.data });
                    seasonResponses.Add(await GetResponseAndUpdateTable(seasonRequest, "C"));                   
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    id = "01",
                    method = "LoadDataSeason",
                    quantity = 0,
                    message = ex.Message,
                    table = false,
                    name = "Season"
                });
            }
            finally
            {
                _commonRepository.Bbook_history("Bbook_Season", JsonConvert.SerializeObject(seasonRequests),
                    JsonConvert.SerializeObject(seasonResponses), JsonConvert.SerializeObject(pruebaInternas));
                _iDBOracleRepository.Dispose();
            }
            return new DTO<Season>()
            {
                requests = seasonRequests,
                responses = seasonResponses,
                pruebaInterna = new PruebaInternaDTO() { data = pruebaInternas }
            };
        }
        public List<Season> GetSeasons(string storedProcedure,string tipoproceso)
        {
            //Busca y carga marcas nuevas a la tabla Book_Season //Consulta todas las marcas en la tabla Book_Season
            List<Season> Seasons = new List<Season>();
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;

            try
            {
                oracleCommand.Parameters.Add("Seasons_C", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand,ref ErrorMessage, storedProcedure, true))
                {
                    if(ErrorMessage == "OK")
                    {
                        while (oracleDataReader.Read())
                        {
                            Seasons.Add(new Season()
                            {
                                id = oracleDataReader["season_id"].ToString().Trim(),
                                name = oracleDataReader["season_name"].ToString().Trim()
                            });
                        }
                    }
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetSeasons"+ tipoproceso,
                        quantity = Seasons.Count,
                        message = ErrorMessage,
                        name = "Season",
                        table = false
                    });
                }
                else
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetSeasons"+ tipoproceso,
                        quantity = 0,
                        message = ErrorMessage,
                        name = "Season",
                        table = false
                    });
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInternaDTO.PruebaInterna()
                {
                    id = "01",
                    method = "GetSeasons"+ tipoproceso,
                    quantity = 0,
                    message = ex.Message,
                    name = "Season",
                    table = false
                });
            }
            return Seasons;
        }
        #endregion
        #region Metodos Extras
        public PruebaInterna DeleteBbook_Season()
        {
            return _commonRepository.DeleteTable("Bbook_Season");
        }
        public async Task<DTO<Season>.Response> GetResponseAndUpdateTable(DTO<Season>.Request seasonRequest, string tipo)
        {
            HttpResponseMessage res;
            DTO<Season>.Response seasonResponse = new DTO<Season>.Response();
            List<string> codigos = new List<string>();
            ExceptionalErrorDTO<Season> exceptionalError;

            res = await _commonRepository.ApiBbook(JsonConvert.SerializeObject(seasonRequest), tipo, "seasons");
            if (res.StatusCode == HttpStatusCode.OK || res.StatusCode == HttpStatusCode.Conflict)
            {
                try
                {
                    seasonResponse = JsonConvert.DeserializeObject<DTO<Season>.Response>(res.Content.ReadAsStringAsync().Result);
                    if(res.StatusCode == HttpStatusCode.OK)
                    {
                        codigos = (from c in seasonRequest.data select c.id).ToList();
                        foreach (var item in _commonRepository.UpdateDateTable("season", tipo, codigos))
                        {
                            pruebaInternas.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        exceptionalError = JsonConvert.DeserializeObject<ExceptionalErrorDTO<Season>>(res.Content.ReadAsStringAsync().Result);
                        return new DTO<Season>.Response()
                        {
                            internalCode = exceptionalError.internalCode,
                            message = exceptionalError.message,
                            statusCode = exceptionalError.statusCode,
                            status = exceptionalError.status
                        };
                    }
                    catch
                    {
                        seasonResponse = new DTO<Season>.Response()
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
                    seasonResponse = new DTO<Season>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 401,
                        status = "error"
                    };
                }
                else if (res.StatusCode == HttpStatusCode.RequestEntityTooLarge)
                {
                    seasonResponse = new DTO<Season>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 413,
                        status = "error"
                    };
                }
                else if (res.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    seasonResponse = new DTO<Season>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 504,
                        status = "error"
                    };
                }
            }
            return seasonResponse;
        }
        #endregion
    }
}
