using Facade.Models.In_Asn;
using IntegracionBbook.Api.Models.In_po;
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
using static IntegracionBbook.Api.Models.In_po.Out_Po;
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Repositories.Repositories
{
    public class In_AsnRepository : IIn_AsnRepository
    {
        string ErrorMessage;
        private readonly IDBOracleRepository _iDBOracleRepository; //Interface de Conexion a Oracle
        private readonly ICommonRepository _commonRepository; //Interface de metodos comunes
        private List<PruebaInterna> pruebaInternas;

        public In_AsnRepository(IDBOracleRepository iDBOracleRepository, ICommonRepository commonRepository)
        {
            _iDBOracleRepository = iDBOracleRepository;
            _commonRepository = commonRepository;
        }

        #region Metodos Principales
        public DTOUnitario<In_Asn>.Response LoadDataIn_Asn(DTOUnitario<In_Asn>.Request in_AsnRequest)
        {
            DTOUnitario<In_Asn>.Response response = new DTOUnitario<In_Asn>.Response();
            List<ErrorDTO<In_Asn>> errors = new List<ErrorDTO<In_Asn>>();
            pruebaInternas = new List<PruebaInterna>();
            if (_commonRepository.DataOKIn_Asn(ref ErrorMessage, in_AsnRequest))
            {
                try
                {
                    //HACER PROCESO 
                    foreach (var item in _commonRepository.AddDataTableIn_Asn("In_Asn", "A", in_AsnRequest))
                    {
                        pruebaInternas.Add(item);
                        if (item.message != "OK")
                        {
                            errors.Add(new ErrorDTO<In_Asn>()
                            {
                                code = "01",
                                message = item.message,
                                record = in_AsnRequest.data
                            });
                        }
                    }
                    if (errors.Count < 1) response = new DTOUnitario<In_Asn>.Response() { status = "OK", statusCode = 200, internalCode = "00", message = "OK" };
                    else response = new DTOUnitario<In_Asn>.Response() { status = "ERROR", statusCode = 409, internalCode = "00", message = "ERROR", errors = errors };
                }
                catch (Exception ex)
                {
                    response = new DTOUnitario<In_Asn>.Response() { status = "ERROR", statusCode = 500, internalCode = "01", message = ex.Message };
                }
                finally
                {
                    _commonRepository.Bbook_history("Bbook_In_Asn_CAB", JsonConvert.SerializeObject(in_AsnRequest),
                   JsonConvert.SerializeObject(response), JsonConvert.SerializeObject(pruebaInternas));
                    _iDBOracleRepository.Dispose();
                }
            }
            else
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    id = in_AsnRequest.data.asn == 0 ? "" : in_AsnRequest.data.asn.ToString(),
                    table = false,
                    message = ErrorMessage,
                    method = "LoadDataIn_Asn",
                    name = "In_po",
                    quantity = 1
                });
                response = new DTOUnitario<In_Asn>.Response()
                {
                    internalCode = "99",
                    message = ErrorMessage,
                    status = "ERROR",
                    statusCode = 406
                };
                _commonRepository.Bbook_history("Bbook_In_Asn_CAB", JsonConvert.SerializeObject(in_AsnRequest),
                   JsonConvert.SerializeObject(response), JsonConvert.SerializeObject(pruebaInternas));
            }

            return response;
        }

        //public async Task<DTO<Out_Po>> SendDataOut_po()
        //{
        //    List<DTO<Out_Po>.Request> requests = new List<DTO<Out_Po>.Request>();
        //    List<DTO<Out_Po>.Response> responses = new List<DTO<Out_Po>.Response>();
        //    DTOUnitario<Out_Po>.Request Out_poRequest = new DTOUnitario<Out_Po>.Request();
        //    DTOUnitario<Out_Po>.Response Out_poResponse = new DTOUnitario<Out_Po>.Response();
        //    pruebaInternas = new List<PruebaInterna>();
        //    try
        //    {
        //        foreach (var item in GetOut_pos("SP_OUT_PO_READ", "-S"))
        //        {
        //            Out_poRequest.data = item;
        //            if (Out_poRequest != null)
        //            {
        //                //HACER PROCESO POST HACIA EL BBOOK
        //                requests.Add(new DTO<Out_Po>.Request()
        //                {
        //                    data = new List<Out_Po>() { Out_poRequest.data },
        //                    MessageError = ""
        //                });
        //                //Out_poRequest = new DTO<Out_Po>.Request() { data = Out_poRequest.data };
        //                responses.Add(await GetResponseAndUpdateTable(Out_poRequest, "A"));
        //            }
        //        }
        //        foreach (var item in GetOut_pos("SP_OUT_ERROR_PO_READ", "-S-E"))
        //        {
        //            Out_poRequest.data = item;
        //            if (Out_poRequest != null)
        //            {
        //                //HACER PROCESO POST HACIA EL BBOOK
        //                requests.Add(new DTO<Out_Po>.Request()
        //                {
        //                    data = new List<Out_Po>() { Out_poRequest.data },
        //                    MessageError = ""
        //                });
        //                //Out_poRequest = new DTO<Out_Po>.Request() { data = Out_poRequest.data };
        //                responses.Add(await GetResponseAndUpdateTable(Out_poRequest, "A"));
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        pruebaInternas.Add(new PruebaInterna()
        //        {
        //            id = "01",
        //            method = "SendDataOut_po",
        //            quantity = 0,
        //            message = ex.Message,
        //            table = false,
        //            name = "Out_po"
        //        });
        //    }
        //    finally
        //    {
        //        _commonRepository.Bbook_history("Bbook_Out_po", JsonConvert.SerializeObject(requests),
        //           JsonConvert.SerializeObject(responses), JsonConvert.SerializeObject(pruebaInternas));
        //        _iDBOracleRepository.Dispose();
        //    }
        //    return new DTO<Out_Po>()
        //    {
        //        requests = requests,
        //        responses = responses,
        //        pruebaInterna = new PruebaInternaDTO() { data = pruebaInternas }
        //    };
        //}

        #endregion

        #region Metodos Extras

        public List<Out_Po> GetOut_pos(string storedProcedure, string tipoproceso)
        {
            //Busca y carga marcas nuevas a la tabla Book_In_po //Consulta todas las marcas en la tabla Book_In_po
            List<Out_Po> Out_pos = new List<Out_Po>();
            List<Out_Po> Out_posTemporal = new List<Out_Po>();
            Out_Po out_PoTemporal = new Out_Po();
            List<Label> labels = new List<Label>();
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;
            OracleDataReader oracleDataReaderEtiquetas = null;
            bool devuelvedatos = false;
            int Codigoactual = 0;

            try
            {
                oracleCommand.Parameters.Add("OUT_POS_C", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, storedProcedure, true))
                {
                    if (ErrorMessage == "OK")
                    {
                        while (oracleDataReader.Read())
                        {
                            devuelvedatos = true;

                            if (Codigoactual != 0 &&
                                Codigoactual != int.Parse(oracleDataReader["id_document"].ToString().Trim()))
                            {
                                if (tipoproceso != "-S-E")
                                {
                                    out_PoTemporal = Out_posTemporal.GroupBy(x => x.id_document)
                                        .Select(group => group.First()).FirstOrDefault();
                                    oracleCommand = new OracleCommand();
                                    oracleCommand.Parameters.Add("purchase_order", OracleDbType.Int32).Value = out_PoTemporal.purchase_order;
                                    oracleCommand.Parameters.Add("OUT_POS_C", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                                    if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReaderEtiquetas, ref oracleCommand, ref ErrorMessage, "SP_OUT_PO_ETIQUETAS_READ", true))
                                    {
                                        while (oracleDataReaderEtiquetas.Read())
                                        {
                                            labels.Add(new Label()
                                            {
                                                Tipo = oracleDataReaderEtiquetas["tipo"].ToString().Trim(),
                                                Numero = oracleDataReaderEtiquetas["numero"].ToString().Trim(),
                                                Etiqueta = oracleDataReaderEtiquetas["etiqueta"].ToString().Trim(),
                                                EAN = oracleDataReaderEtiquetas["ean"].ToString().Trim(),
                                                Desc1 = oracleDataReaderEtiquetas["desc1"].ToString().Trim(),
                                                Desc2 = oracleDataReaderEtiquetas["desc2"].ToString().Trim(),
                                                Dpto = oracleDataReaderEtiquetas["dpto"].ToString().Trim(),
                                                Linea = oracleDataReaderEtiquetas["linea"].ToString().Trim(),
                                                Moneda = oracleDataReaderEtiquetas["moneda"].ToString().Trim(),
                                                Precio = oracleDataReaderEtiquetas["precio"].ToString().Trim(),
                                                Sku = oracleDataReaderEtiquetas["sku"].ToString().Trim(),
                                                Marca = oracleDataReaderEtiquetas["marca"].ToString().Trim(),
                                                STQty = oracleDataReaderEtiquetas["qty"].ToString().Trim(),
                                                Temporada = oracleDataReaderEtiquetas["temporada"].ToString().Trim()
                                            });
                                        }
                                    }
                                    Out_pos.Add(new Out_Po()
                                    {
                                        id_document = out_PoTemporal.id_document,
                                        purchase_order = out_PoTemporal.purchase_order,
                                        label = labels,
                                        status = out_PoTemporal.status,
                                        error = out_PoTemporal.error
                                    });
                                    Out_posTemporal = new List<Out_Po>();
                                    labels = new List<Label>();
                                }
                            }
                            Out_posTemporal.Add(new Out_Po()
                            {
                                id_document = int.Parse(oracleDataReader["id_document"].ToString().Trim()),
                                purchase_order = oracleDataReader["purchase_order"].ToString().Trim(),
                                status = int.Parse(oracleDataReader["err_code"].ToString().Trim()),
                                error = oracleDataReader["err_desc"].ToString().Trim()
                            });
                            Codigoactual = int.Parse(oracleDataReader["id_document"].ToString().Trim());
                        }
                        if (Out_posTemporal.Count > 0 && devuelvedatos == true)
                        {
                            if (tipoproceso != "-S-E")
                            {
                                out_PoTemporal = Out_posTemporal.GroupBy(x => x.id_document)
                                        .Select(group => group.First()).FirstOrDefault();
                                oracleCommand = new OracleCommand();
                                oracleCommand.Parameters.Add("purchase_order", OracleDbType.Int32).Value = out_PoTemporal.purchase_order;
                                oracleCommand.Parameters.Add("OUT_POS_C", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReaderEtiquetas, ref oracleCommand, ref ErrorMessage, "SP_OUT_PO_ETIQUETAS_READ", true))
                                {
                                    while (oracleDataReaderEtiquetas.Read())
                                    {
                                        labels.Add(new Label()
                                        {
                                            Tipo = oracleDataReaderEtiquetas["tipo"].ToString().Trim(),
                                            Numero = oracleDataReaderEtiquetas["numero"].ToString().Trim(),
                                            Etiqueta = oracleDataReaderEtiquetas["etiqueta"].ToString().Trim(),
                                            EAN = oracleDataReaderEtiquetas["ean"].ToString().Trim(),
                                            Desc1 = oracleDataReaderEtiquetas["desc1"].ToString().Trim(),
                                            Desc2 = oracleDataReaderEtiquetas["desc2"].ToString().Trim(),
                                            Dpto = oracleDataReaderEtiquetas["dpto"].ToString().Trim(),
                                            Linea = oracleDataReaderEtiquetas["linea"].ToString().Trim(),
                                            Moneda = oracleDataReaderEtiquetas["moneda"].ToString().Trim(),
                                            Precio = oracleDataReaderEtiquetas["precio"].ToString().Trim(),
                                            Sku = oracleDataReaderEtiquetas["sku"].ToString().Trim(),
                                            Marca = oracleDataReaderEtiquetas["marca"].ToString().Trim(),
                                            STQty = oracleDataReaderEtiquetas["qty"].ToString().Trim(),
                                            Temporada = oracleDataReaderEtiquetas["temporada"].ToString().Trim()
                                        });
                                    }
                                }
                                Out_pos.Add(new Out_Po()
                                {
                                    id_document = out_PoTemporal.id_document,
                                    purchase_order = out_PoTemporal.purchase_order,
                                    label = labels,
                                    status = out_PoTemporal.status,
                                    error = out_PoTemporal.error
                                });
                                Out_posTemporal = new List<Out_Po>();
                                labels = new List<Label>();
                            }
                            else
                            {
                                foreach (var item in Out_posTemporal)
                                {
                                    Out_pos.Add(item);
                                }
                            }
                        }
                    }
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetOut_po" + tipoproceso,
                        quantity = Out_pos.Count,
                        message = ErrorMessage,
                        name = "Out_po_cab",
                        table = false
                    });
                }
                else
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetOut_po" + tipoproceso,
                        quantity = 0,
                        message = ErrorMessage,
                        name = "Out_po_cab",
                        table = false
                    });
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    id = "01",
                    method = "GetOut_po" + tipoproceso,
                    quantity = 0,
                    message = ex.Message,
                    name = "Out_po_cab",
                    table = false
                });
            }
            return Out_pos;
        }

        public async Task<DTO<Out_Po>.Response> GetResponseAndUpdateTable(DTOUnitario<Out_Po>.Request Out_poRequest, string tipo)
        {
            HttpResponseMessage res;
            DTO<Out_Po>.Response out_poResponse = new DTO<Out_Po>.Response();
            List<string> codigos = new List<string>();
            ExceptionalErrorDTO<In_Po> exceptionalError;
            try
            {
                res = await _commonRepository.ApiBbook(JsonConvert.SerializeObject(Out_poRequest), tipo, "in-po");
                if (res.Content != null)
                {
                    if (res.StatusCode == HttpStatusCode.OK || res.StatusCode == HttpStatusCode.Conflict)
                    {
                        try
                        {
                            out_poResponse = JsonConvert.DeserializeObject<DTO<Out_Po>.Response>(res.Content.ReadAsStringAsync().Result);
                            if (res.StatusCode == HttpStatusCode.OK)
                            {
                                foreach (var item in _commonRepository.UpdateDateTableIn_po("in_po", tipo, Out_poRequest.data.purchase_order.ToString()))
                                {
                                    pruebaInternas.Add(item);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                exceptionalError = JsonConvert.DeserializeObject<ExceptionalErrorDTO<In_Po>>(res.Content.ReadAsStringAsync().Result);
                                return new DTO<Out_Po>.Response()
                                {
                                    internalCode = exceptionalError.internalCode,
                                    message = exceptionalError.message,
                                    statusCode = exceptionalError.statusCode,
                                    status = exceptionalError.status
                                };
                            }
                            catch
                            {
                                out_poResponse = new DTO<Out_Po>.Response()
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
                            out_poResponse = new DTO<Out_Po>.Response()
                            {
                                internalCode = res.StatusCode.ToString(),
                                message = res.Content.ReadAsStringAsync().Result,
                                statusCode = 401,
                                status = "error"
                            };
                        }
                        else if (res.StatusCode == HttpStatusCode.RequestEntityTooLarge)
                        {
                            out_poResponse = new DTO<Out_Po>.Response()
                            {
                                internalCode = res.StatusCode.ToString(),
                                message = res.Content.ReadAsStringAsync().Result,
                                statusCode = 413,
                                status = "error"
                            };
                        }
                    }
                    out_poResponse.bulkdata = res.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    out_poResponse = new DTO<Out_Po>.Response()
                    {
                        internalCode = "500",
                        message = "Endpoint in-po no encontrado",
                        statusCode = 413,
                        status = "error"
                    };
                }
            }
            catch (Exception ex)
            {
                out_poResponse = new DTO<Out_Po>.Response()
                {
                    internalCode = "500",
                    message = ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.Source,
                    statusCode = 413,
                    status = "error"
                };
            }

            return out_poResponse;
        }

        public PruebaInternaDTO DeleteBbook_In_Asn()
        {
            return null;
            /*
            return new PruebaInternaDTO()
            {
                data = new List<PruebaInterna>()
                {
                    _commonRepository.DeleteTable("Bbook_in_po_cab"),
                    _commonRepository.DeleteTable("Bbook_in_po_det")
                }
            };
            */
        }

        private string ObtenerDescripcionOutPO(string strDescripcion, int tipoDesc)
        {
            int maxLenght = 24;
            switch (tipoDesc)
            {
                case 1:
                    strDescripcion = (strDescripcion.Length > maxLenght ? strDescripcion.Substring(0, maxLenght) : strDescripcion);
                    break;
                case 2:
                    strDescripcion = (strDescripcion.Length > maxLenght ? strDescripcion.Substring(maxLenght, strDescripcion.Length - maxLenght) : "");
                    break;
                default:
                    return "";
            }
            return strDescripcion;
        }
        #endregion
    }
}
