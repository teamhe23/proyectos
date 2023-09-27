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
    public class Master_poRepository: IMaster_poRepository
    {
        string ErrorMessage;
        private readonly IDBOracleRepository _iDBOracleRepository; //Interface de Conexion a Oracle
        private readonly ICommonRepository _commonRepository; //Interface de metodos comunes
        private List<PruebaInterna> pruebaInternas;
        public Master_poRepository(IDBOracleRepository iDBOracleRepository, ICommonRepository commonRepository)
        {
            _iDBOracleRepository = iDBOracleRepository;
            _commonRepository = commonRepository;
        }
        #region Metodos Principales
        public DTO<Master_po>.Request GetAllMaster_pos(bool subproceso)
        {
            pruebaInternas = new List<PruebaInterna>();
            try
            {
                return new DTO<Master_po>.Request()
                {
                    data = GetMaster_pos("sp_master_po_read",""),
                    MessageError = pruebaInternas[0].message
                };
            }
            catch (Exception ex)
            {
                return new DTO<Master_po>.Request()
                {
                    MessageError = ex.Message
                };
            }
            finally
            {
                if (!subproceso) _iDBOracleRepository.Dispose();
            }
        }
        public async Task<DTO<Master_po>> LoadDataMaster_po()
        {
            List<DTO<Master_po>.Request> master_poRequests = new List<DTO<Master_po>.Request>();
            List<DTO<Master_po>.Response> master_poResponses = new List<DTO<Master_po>.Response>();
            DTO<Master_po>.Request master_poRequest = new DTO<Master_po>.Request();
            IEnumerable<IEnumerable<Master_po>> master_PoRequestTemporal;
            pruebaInternas = new List<PruebaInterna>();
            try
            {
                //master_poRequest = GetAllMaster_pos(true);
                //if (master_poRequest.data.Count == 0) master_poRequest.data = GetMaster_pos("sp_master_po_load", "-L");
                //else
                //{
                //    master_poRequest.data = GetMaster_pos("sp_master_po_create", "-A");
                //}
                master_poRequest.data = GetMaster_pos("sp_master_po_create", "-A");

                if (master_poRequest.data.Count != 0)
                {
                    //HACER PROCESO POST HACIA EL BBOOK
                    master_poRequests.Add(new DTO<Master_po>.Request() { data = master_poRequest.data });
                    if (master_poRequest.data.Count >= 2000)
                    {
                        master_PoRequestTemporal = LinqExtensions.Split(master_poRequest.data, master_poRequest.data.Count / 500);
                        foreach (var item in master_PoRequestTemporal)
                        {
                            master_poRequest.data = (List<Master_po>)item.ToList();
                            master_poResponses.Add(await GetResponseAndUpdateTable(master_poRequest, "A"));
                        }
                    }
                    else master_poResponses.Add(await GetResponseAndUpdateTable(master_poRequest, "A"));
                }
                //master_poRequest.data = GetMaster_pos("sp_master_po_update");
                //if (master_poRequest.data.Count != 0)
                //{
                //    //HACER PROCESO PUT HACIA EL BBOOK
                //    master_poRequests.Add(new DTO<Master_po>.Request() { data = master_poRequest.data });
                //    master_poResponses.Add(await GetResponseAndUpdateTable(master_poRequest, "C"));
                //}
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    id = "01",
                    method = "LoadDataMaster_po",
                    quantity = 0,
                    message = ex.Message,
                    table = false,
                    name = "Master_po"
                });
            }
            finally
            {
                _commonRepository.Bbook_history("Bbook_Master_po", JsonConvert.SerializeObject(master_poRequests),
                    JsonConvert.SerializeObject(master_poResponses), JsonConvert.SerializeObject(pruebaInternas));
                _iDBOracleRepository.Dispose();
            }
            return new DTO<Master_po>()
            {
                requests = master_poRequests,
                responses = master_poResponses,
                pruebaInterna = new PruebaInternaDTO() { data = pruebaInternas }
            };
        }
        #endregion
        #region Metodos Extras
        public List<Master_po> GetMaster_pos(string storedProcedure,string tipoproceso)
        {
            //Busca y carga marcas nuevas a la tabla Book_Master_po //Consulta todas las marcas en la tabla Book_Master_po
            List<Master_po> Master_pos = new List<Master_po>();
            List<string> skus = new List<string>(); 
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;
            int units=0;
            double unit_cost=0;

            try
            {
                oracleCommand.Parameters.Add("Master_pos_C", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, storedProcedure, true))
                {
                    if (ErrorMessage == "OK") 
                    { 
                        while (oracleDataReader.Read())
                        {
                            Master_pos.Add(new Master_po()
                            {
                                Master_po_id = oracleDataReader["master_po_id"].ToString().Trim(),
                                purchase_order = oracleDataReader["purchase_order"].ToString().Trim(),
                                vendor_id = oracleDataReader["vendor_id"].ToString().Trim(),
                                currency = oracleDataReader["currency"].ToString().Trim(),
                                incoterm = oracleDataReader["incoterm"].ToString().Trim(),
                                port_of_loading = oracleDataReader["port_of_loading"].ToString().Trim(),
                                port_of_discharge = oracleDataReader["port_of_discharge"].ToString().Trim(),
                                payment_terms = oracleDataReader["payment_terms"].ToString().Trim(),
                                creation_date = _commonRepository.CambiarFormatoFecha(oracleDataReader["creation_date"].ToString())
                            });
                            if (oracleDataReader["units"].ToString().Trim() != string.Empty) units = int.Parse(oracleDataReader["units"].ToString().Trim());
                            if (oracleDataReader["unit_cost"].ToString().Trim() != string.Empty) unit_cost = double.Parse(oracleDataReader["unit_cost"].ToString().Trim());
                            //skus.Add(oracleDataReader["master_po_id"].ToString().Trim() + "-" + units.ToString() + "-" + unit_cost.ToString());
                            skus.Add(string.Concat(oracleDataReader["master_po_id"].ToString().Trim() , "-", units.ToString(), "-", unit_cost.ToString()));
                        }
                        Master_pos = Master_pos.GroupBy(x => x.purchase_order)
                         .Select(group => group.First()).ToList();
                        for (int i = 0; i < Master_pos.Count; i++)
                        {
                            Master_pos[i].details = (from x in skus where x.Split("-")[0] == Master_pos[i].purchase_order
                                                     select new Master_po.SKU()
                                                     {
                                                         sku = x.Split("-")[1],
                                                         units = int.Parse(x.Split("-")[2]),
                                                         unit_cost = double.Parse(x.Split("-")[3])
                                                     }).ToList();
                        } 
                    }
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetMaster_pos" + tipoproceso,
                        quantity = Master_pos.Count,
                        message = ErrorMessage,
                        name = "Master_po",
                        table = false
                    });
                }
                else
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetMaster_pos"+ tipoproceso,
                        quantity = 0,
                        message = ErrorMessage,
                        name = "Master_po",
                        table = false
                    });
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInternaDTO.PruebaInterna()
                {
                    id = "01",
                    method = "GetMaster_pos"+ tipoproceso,
                    quantity = 0,
                    message = ex.Message,
                    name = "Master_po",
                    table = false
                });
            }
            return Master_pos;
        }
        public async Task<DTO<Master_po>.Response> GetResponseAndUpdateTable(DTO<Master_po>.Request master_poRequest, string tipo)
        {
            HttpResponseMessage res = new HttpResponseMessage();
            DTO<Master_po>.Response master_poResponse = new DTO<Master_po>.Response();
            List<string> codigos = new List<string>();
            ExceptionalErrorDTO<Master_po> exceptionalError;

            res = await _commonRepository.ApiBbook(JsonConvert.SerializeObject(master_poRequest), tipo, "master-po");
            if (res.StatusCode == HttpStatusCode.OK || res.StatusCode == HttpStatusCode.Conflict)
            {
                try
                {
                    master_poResponse = JsonConvert.DeserializeObject<DTO<Master_po>.Response>(res.Content.ReadAsStringAsync().Result);
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        codigos = (from c in master_poRequest.data select c.purchase_order).ToList();
                        foreach (var item in _commonRepository.UpdateDateTableMaster_po("master_po", tipo, codigos))
                        {
                            pruebaInternas.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        exceptionalError = JsonConvert.DeserializeObject<ExceptionalErrorDTO<Master_po>>(res.Content.ReadAsStringAsync().Result);
                        return new DTO<Master_po>.Response()
                        {
                            internalCode = exceptionalError.internalCode,
                            message = exceptionalError.message,
                            statusCode = exceptionalError.statusCode,
                            status = exceptionalError.status
                        };
                    }
                    catch
                    {
                        master_poResponse = new DTO<Master_po>.Response()
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
                    master_poResponse = new DTO<Master_po>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 401,
                        status = "error"
                    };
                }
                else if (res.StatusCode == HttpStatusCode.RequestEntityTooLarge)
                {
                    master_poResponse = new DTO<Master_po>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 413,
                        status = "error"
                    };
                }
                else if (res.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    master_poResponse = new DTO<Master_po>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 504,
                        status = "error"
                    };
                }
            }
            return master_poResponse;
        }
        public PruebaInterna DeleteBbook_Master_po()
        {
            return _commonRepository.DeleteTable("Bbook_master_po");
        }
        #endregion
    }
}
