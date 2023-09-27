using IntegracionBbook.Api.Models.In_Codes;
using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Data.Interfaces;
using IntegracionBbook.Models;
using IntegracionBbook.Repositories.Interfaces;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static IntegracionBbook.Api.Models.In_Codes.Out_Codes;
using static IntegracionBbook.Api.Models.In_Codes.Out_Codes.DimValueOut;
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Repositories.Repositories
{
    public class In_CodesRepository : IIn_CodesRepository
    {
        string ErrorMessage;
        private readonly IDBOracleRepository _iDBOracleRepository; //Interface de Conexion a Oracle
        private readonly ICommonRepository _commonRepository; //Interface de metodos comunes
        private List<PruebaInterna> pruebaInternas = new List<PruebaInterna>();

        public In_CodesRepository(IDBOracleRepository iDBOracleRepository, ICommonRepository commonRepository)
        {
            _iDBOracleRepository = iDBOracleRepository;
            _commonRepository = commonRepository;
        }

        #region Metodos Principales

        //public string GetAllIn_Codes()
        //{
        //    throw new NotImplementedException();
        //}

        //public DTOUnitario<In_Codes>.Response LoadDataIn_Codes(DTOUnitario<In_Codes>.Request in_CodesRequest, ref PruebaInterna pruebaInterna)
        //{
        //    List<DTOUnitario<In_Codes>.Request> requests = new List<DTOUnitario<In_Codes>.Request>();
        //    List<DTOUnitario<In_Codes>.Response> responses = new List<DTOUnitario<In_Codes>.Response>();

        //    requests.Add(in_CodesRequest);
        //    DTOUnitario<In_Codes>.Response response = new DTOUnitario<In_Codes>.Response();
        //    List<ErrorDTO<In_Codes>> errors = new List<ErrorDTO<In_Codes>>();
        //    pruebaInternas = new List<PruebaInterna>();
        //    try
        //    {
        //        if (in_CodesRequest.data.gs_attrs == null)
        //        {
        //            In_CodesNoUniqueSize in_CodesNoUniqueSize =
        //                JsonConvert.DeserializeObject<In_CodesNoUniqueSize>(JsonConvert.SerializeObject(in_CodesRequest));

        //            if (_commonRepository.DataOKIn_Codes(ref ErrorMessage, in_CodesRequest, 1))
        //            {
        //                try
        //                {
        //                    foreach (var item in _commonRepository.AddDataTableIn_Codes("In_Codes", in_CodesRequest, 1))
        //                    {
        //                        pruebaInternas.Add(item);
        //                        if (item.message != "OK")
        //                        {
        //                            errors.Add(new ErrorDTO<In_Codes>()
        //                            {
        //                                code = "01",
        //                                message = item.message,
        //                                record = in_CodesRequest.data
        //                            });
        //                        }
        //                    }
        //                    if (errors.Count < 1) response = new DTOUnitario<In_Codes>.Response()
        //                    { status = "OK", statusCode = 200, internalCode = "00", message = "OK" };
        //                    else response = new DTOUnitario<In_Codes>.Response()
        //                    { status = "ERROR", statusCode = 409, internalCode = "00", message = "ERROR", errors = errors };
        //                    responses.Add(response);
        //                }
        //                catch (Exception ex)
        //                {
        //                    response = new DTOUnitario<In_Codes>.Response()
        //                    { status = "ERROR", statusCode = 500, internalCode = "01", message = ex.Message };
        //                    responses.Add(response);
        //                }
        //            }
        //            else
        //            {
        //                response = new DTOUnitario<In_Codes>.Response()
        //                {
        //                    internalCode = "99",
        //                    message = ErrorMessage,
        //                    status = "ERROR",
        //                    statusCode = 406
        //                };
        //                responses.Add(response);
        //            }
        //        }
        //        else
        //        {
        //            In_CodesUniqueSize in_CodesUniqueSize =
        //                JsonConvert.DeserializeObject<In_CodesUniqueSize>(JsonConvert.SerializeObject(in_CodesRequest));

        //            if (_commonRepository.DataOKIn_Codes(ref ErrorMessage, in_CodesRequest, 2))
        //            {
        //                try
        //                {
        //                    foreach (var item in _commonRepository.AddDataTableIn_Codes("In_Codes", in_CodesRequest, 2))
        //                    {
        //                        pruebaInternas.Add(item);
        //                        if (item.message != "OK")
        //                        {
        //                            errors.Add(new ErrorDTO<In_Codes>()
        //                            {
        //                                code = "01",
        //                                message = item.message,
        //                                record = in_CodesRequest.data
        //                            });
        //                        }
        //                    }
        //                    if (errors.Count < 1) response = new DTOUnitario<In_Codes>.Response()
        //                    { status = "OK", statusCode = 200, internalCode = "00", message = "OK" };
        //                    else response = new DTOUnitario<In_Codes>.Response()
        //                    { status = "ERROR", statusCode = 409, internalCode = "00", message = "ERROR", errors = errors };
        //                    responses.Add(response);
        //                }
        //                catch (Exception ex)
        //                {
        //                    response = new DTOUnitario<In_Codes>.Response()
        //                    { status = "ERROR", statusCode = 500, internalCode = "01", message = ex.Message };
        //                    responses.Add(response);
        //                }
        //            }
        //            else
        //            {
        //                response = new DTOUnitario<In_Codes>.Response()
        //                {
        //                    internalCode = "99",
        //                    message = ErrorMessage,
        //                    status = "ERROR",
        //                    statusCode = 406
        //                };
        //                responses.Add(response);
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        response = new DTOUnitario<In_Codes>.Response()
        //        {
        //            internalCode = "00",
        //            message = "Error de conversion del JSON",
        //            status = "ERROR",
        //            statusCode = 500
        //        };
        //        responses.Add(response);
        //    }
        //    finally
        //    {
        //        _commonRepository.Bbook_history("Bbook_In_codes", JsonConvert.SerializeObject(requests),
        //                    JsonConvert.SerializeObject(responses), JsonConvert.SerializeObject(pruebaInternas));
        //        requests = new List<DTOUnitario<In_Codes>.Request>();
        //        responses = new List<DTOUnitario<In_Codes>.Response>();
        //    }
        //    return response;
        //}
        public DTOUnitario<In_Codes>.Response LoadDataIn_Codes(DTOUnitario<In_Codes>.Request in_CodesRequest, ref PruebaInterna pruebaInterna)
        {
            List<DTOUnitario<In_Codes>.Request> requests = new List<DTOUnitario<In_Codes>.Request>();
            List<DTOUnitario<In_Codes>.Response> responses = new List<DTOUnitario<In_Codes>.Response>();

            requests.Add(in_CodesRequest);
            DTOUnitario<In_Codes>.Response response = new DTOUnitario<In_Codes>.Response();
            List<ErrorDTO<In_Codes>> errors = new List<ErrorDTO<In_Codes>>();
            pruebaInternas = new List<PruebaInterna>();
            try
            {
                Boolean valido = false;
                if (in_CodesRequest.data.is_unique_size == false)
                {
                    valido = _commonRepository.DataOKIn_Codes(ref ErrorMessage, in_CodesRequest, 1);
                }
                else
                {
                    valido = _commonRepository.DataOKIn_Codes(ref ErrorMessage, in_CodesRequest, 2);
                }

                if (valido)
                {
                    try
                    {
                        foreach (var item in _commonRepository.AddDataTableIn_Codes("In_Codes", in_CodesRequest, 1))
                        {
                            pruebaInternas.Add(item);
                            if (item.message != "OK")
                            {
                                errors.Add(new ErrorDTO<In_Codes>()
                                {
                                    code = "01",
                                    message = item.message,
                                    record = in_CodesRequest.data
                                });
                            }
                        }
                        if (errors.Count < 1) response = new DTOUnitario<In_Codes>.Response()
                        { status = "OK", statusCode = 200, internalCode = "00", message = "OK" };
                        else response = new DTOUnitario<In_Codes>.Response()
                        { status = "ERROR", statusCode = 409, internalCode = "00", message = "ERROR", errors = errors };
                        responses.Add(response);
                    }
                    catch (Exception ex)
                    {
                        response = new DTOUnitario<In_Codes>.Response()
                        { status = "ERROR", statusCode = 500, internalCode = "01", message = ex.Message };
                        responses.Add(response);
                    }
                }
                else
                {
                    response = new DTOUnitario<In_Codes>.Response()
                    {
                        internalCode = "99",
                        message = ErrorMessage,
                        status = "ERROR",
                        statusCode = 406
                    };
                    responses.Add(response);
                }

                //if (in_CodesRequest.data.is_unique_size == false)
                //{
                //    In_CodesNoUniqueSize in_CodesNoUniqueSize =
                //        JsonConvert.DeserializeObject<In_CodesNoUniqueSize>(JsonConvert.SerializeObject(in_CodesRequest));

                //    if (_commonRepository.DataOKIn_Codes(ref ErrorMessage, in_CodesRequest, 1))
                //    {
                //        try
                //        {
                //            foreach (var item in _commonRepository.AddDataTableIn_Codes("In_Codes", in_CodesRequest, 1))
                //            {
                //                pruebaInternas.Add(item);
                //                if (item.message != "OK")
                //                {
                //                    errors.Add(new ErrorDTO<In_Codes>()
                //                    {
                //                        code = "01",
                //                        message = item.message,
                //                        record = in_CodesRequest.data
                //                    });
                //                }
                //            }
                //            if (errors.Count < 1) response = new DTOUnitario<In_Codes>.Response()
                //            { status = "OK", statusCode = 200, internalCode = "00", message = "OK" };
                //            else response = new DTOUnitario<In_Codes>.Response()
                //            { status = "ERROR", statusCode = 409, internalCode = "00", message = "ERROR", errors = errors };
                //            responses.Add(response);
                //        }
                //        catch (Exception ex)
                //        {
                //            response = new DTOUnitario<In_Codes>.Response()
                //            { status = "ERROR", statusCode = 500, internalCode = "01", message = ex.Message };
                //            responses.Add(response);
                //        }
                //    }
                //    else
                //    {
                //        response = new DTOUnitario<In_Codes>.Response()
                //        {
                //            internalCode = "99",
                //            message = ErrorMessage,
                //            status = "ERROR",
                //            statusCode = 406
                //        };
                //        responses.Add(response);
                //    }
                //}
                //else
                //{
                //    In_CodesUniqueSize in_CodesUniqueSize =
                //        JsonConvert.DeserializeObject<In_CodesUniqueSize>(JsonConvert.SerializeObject(in_CodesRequest));

                //    if (_commonRepository.DataOKIn_Codes(ref ErrorMessage, in_CodesRequest, 2))
                //    {
                //        try
                //        {
                //            foreach (var item in _commonRepository.AddDataTableIn_Codes("In_Codes", in_CodesRequest, 2))
                //            {
                //                pruebaInternas.Add(item);
                //                if (item.message != "OK")
                //                {
                //                    errors.Add(new ErrorDTO<In_Codes>()
                //                    {
                //                        code = "01",
                //                        message = item.message,
                //                        record = in_CodesRequest.data
                //                    });
                //                }
                //            }
                //            if (errors.Count < 1) response = new DTOUnitario<In_Codes>.Response()
                //            { status = "OK", statusCode = 200, internalCode = "00", message = "OK" };
                //            else response = new DTOUnitario<In_Codes>.Response()
                //            { status = "ERROR", statusCode = 409, internalCode = "00", message = "ERROR", errors = errors };
                //            responses.Add(response);
                //        }
                //        catch (Exception ex)
                //        {
                //            response = new DTOUnitario<In_Codes>.Response()
                //            { status = "ERROR", statusCode = 500, internalCode = "01", message = ex.Message };
                //            responses.Add(response);
                //        }
                //    }
                //    else
                //    {
                //        response = new DTOUnitario<In_Codes>.Response()
                //        {
                //            internalCode = "99",
                //            message = ErrorMessage,
                //            status = "ERROR",
                //            statusCode = 406
                //        };
                //        responses.Add(response);
                //    }
                //}
            }
            catch
            {
                response = new DTOUnitario<In_Codes>.Response()
                {
                    internalCode = "00",
                    message = "Error de conversion del JSON",
                    status = "ERROR",
                    statusCode = 500
                };
                responses.Add(response);
            }
            finally
            {
                _commonRepository.Bbook_history("Bbook_In_codes", JsonConvert.SerializeObject(requests),
                            JsonConvert.SerializeObject(responses), JsonConvert.SerializeObject(pruebaInternas));
                requests = new List<DTOUnitario<In_Codes>.Request>();
                responses = new List<DTOUnitario<In_Codes>.Response>();
            }
            return response;
        }

        public async Task<DTO<Out_Codes>> SendDataOut_Codes()
        {
            List<DTO<Out_Codes>.Request> requests = new List<DTO<Out_Codes>.Request>();
            List<DTO<Out_Codes>.Response> responses = new List<DTO<Out_Codes>.Response>();
            DTOUnitario<Out_Codes>.Response out_CodesResponses = new DTOUnitario<Out_Codes>.Response();
            DTOUnitario<Out_Codes>.Request Out_CodesRequest = new DTOUnitario<Out_Codes>.Request();
            pruebaInternas = new List<PruebaInterna>();
            try
            {
                //Varias Tallas
                foreach (var item in GetOut_Codes("SP_OUT_CODES_READ", "-S-NUS", false))
                {
                    Out_CodesRequest.data = item;
                    if (Out_CodesRequest.data != null)
                    {
                        //HACER PROCESO POST HACIA EL BBOOK
                        requests.Add(new DTO<Out_Codes>.Request()
                        {
                            data = new List<Out_Codes>() { Out_CodesRequest.data },
                            MessageError = ""
                        });
                        responses.Add(await GetResponseAndUpdateTable(Out_CodesRequest, "S"));
                    }
                }
                //Talla unica
                foreach (var item in GetOut_Codes("SP_OUT_CODES_READ", "-S-US", true))
                {
                    Out_CodesRequest.data = (item);
                    if (Out_CodesRequest.data != null)
                    {
                        //HACER PROCESO POST HACIA EL BBOOK
                        requests.Add(new DTO<Out_Codes>.Request()
                        {
                            data = new List<Out_Codes>() { Out_CodesRequest.data },
                            MessageError = ""
                        });
                        responses.Add(await GetResponseAndUpdateTable(Out_CodesRequest, "S"));
                    }
                }
                foreach (var item in GetOut_Codes("SP_OUT_ERROR_CODES_READ", "-S-E", true))
                {
                    Out_CodesRequest.data = (item);
                    if (Out_CodesRequest.data != null)
                    {
                        //HACER PROCESO POST HACIA EL BBOOK
                        requests.Add(new DTO<Out_Codes>.Request()
                        {
                            data = new List<Out_Codes>() { Out_CodesRequest.data },
                            MessageError = ""
                        });
                        responses.Add(await GetResponseAndUpdateTable(Out_CodesRequest, "S"));
                    }
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    id = "01",
                    method = "SendDataOut_Codes",
                    quantity = 0,
                    message = ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + ErrorMessage,
                    table = false,
                    name = "Out_Codes"
                });
            }
            finally
            {
                _commonRepository.Bbook_history("Bbook_Out_codes", JsonConvert.SerializeObject(requests),
                            JsonConvert.SerializeObject(responses), JsonConvert.SerializeObject(pruebaInternas));
                _iDBOracleRepository.Dispose();
            }
            return new DTO<Out_Codes>()
            {
                requests = requests,
                responses = responses,
                pruebaInterna = new PruebaInternaDTO() { data = pruebaInternas }
            };
        }

        #endregion Metodos Principales

        #region Metodos Extras

        //public List<Out_Codes> GetOut_Codes(string storedProcedure, string tipoproceso, bool is_unique_size)
        //{ 
        //    //Busca y carga marcas nuevas a la tabla Book_In_Codes //Consulta todas las marcas en la tabla Book_In_Codes
        //    List<Out_Codes> Out_Codes = new List<Out_Codes>();
        //    List<Out_Codes> Out_CodesTemporal = new List<Out_Codes>();
        //    Out_Codes Out_CodeTemporal = new Out_Codes();
        //    List<DimValueOut> dimValues = new List<DimValueOut>();
        //    List<SizeOut> sizes = new List<SizeOut>();
        //    OracleCommand oracleCommand = new OracleCommand();
        //    OracleDataReader oracleDataReader = null;
        //    int Codigoactual = 0;
        //    bool devuelvedatos = false;

        //    try
        //    {
        //        if(tipoproceso != "-S-E") oracleCommand.Parameters.Add("is_unique_size", OracleDbType.Varchar2).Value = is_unique_size == true ? "1" : "0";
        //        oracleCommand.Parameters.Add("Out_Codes_C", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        //        if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, storedProcedure, true))
        //        {
        //            if(ErrorMessage == "OK")
        //            {
        //                while (oracleDataReader.Read())
        //                {
        //                    devuelvedatos = true;
        //                    if (tipoproceso != "-S-E" && is_unique_size == false)
        //                    {
        //                        if (Codigoactual != 0 && Codigoactual != int.Parse(oracleDataReader["master_code"].ToString().Trim()))
        //                        {
        //                            Out_CodeTemporal = Out_CodesTemporal.GroupBy(x => x.master_code)
        //                            .Select(group => group.First()).FirstOrDefault();
        //                            dimValues = dimValues.GroupBy(x => x.dimension_id)
        //                            .Select(group => group.First()).ToList();
        //                            for (int i = 0; i < dimValues.Count; i++)
        //                            {
        //                                dimValues[i].sizes = sizes.Select(size => size).Where(size => size.dimCode == dimValues[i].dimension_id).ToList();
        //                            }
        //                            Out_CodeTemporal.dim_values = dimValues;
        //                            dimValues = new List<DimValueOut>();
        //                            sizes = new List<SizeOut>();
        //                            Out_Codes.Add(Out_CodeTemporal);
        //                            Out_CodesTemporal = new List<Out_Codes>();
        //                        }

        //                        dimValues.Add(new DimValueOut()
        //                        {
        //                            dimension_id = oracleDataReader["dim_values_id"].ToString().Trim(),
        //                            style_prepack_code = oracleDataReader["dim_values_STYLE_Prepack_code"].ToString().Trim()
        //                        });
        //                        sizes.Add(new SizeOut()
        //                        {
        //                            dimCode = oracleDataReader["dim_values_id"].ToString().Trim(),
        //                            size = oracleDataReader["dim_values_sizes_size"].ToString().Trim(),
        //                            sku = oracleDataReader["dim_values_sizes_sku"].ToString().Trim(),
        //                            desc = oracleDataReader["dim_values_sizes_desc"].ToString().Trim(),
        //                            ean_upc = oracleDataReader["dim_values_ean_upc"].ToString().Trim()

        //                        });
        //                    }
        //                    Out_CodesTemporal.Add(new Out_Codes()
        //                    {
        //                        master_code = int.Parse(oracleDataReader["master_code"].ToString().Trim()),
        //                        parent_sku = oracleDataReader["parent_sku"].ToString().Trim(),
        //                        status = int.Parse(oracleDataReader["Cod_Error"].ToString().Trim()),
        //                        error = oracleDataReader["Des_error"].ToString().Trim()
        //                    });
        //                    Codigoactual = int.Parse(oracleDataReader["master_code"].ToString().Trim());
        //                }
        //                if (Out_CodesTemporal.Count > 0 && devuelvedatos == true)
        //                {
        //                    if (tipoproceso != "-S-E")
        //                    {
        //                        if (!is_unique_size)
        //                        {
        //                            Out_CodeTemporal = Out_CodesTemporal.GroupBy(x => x.master_code)
        //                            .Select(group => group.First()).FirstOrDefault();
        //                            dimValues = dimValues.GroupBy(x => x.dimension_id)
        //                                .Select(group => group.First()).ToList();
        //                            for (int i = 0; i < dimValues.Count; i++)
        //                            {
        //                                dimValues[i].sizes = sizes.Select(size => size).Where(size => size.dimCode == dimValues[i].dimension_id).ToList();
        //                            }
        //                            Out_CodeTemporal.dim_values = dimValues;
        //                            Out_Codes.Add(Out_CodeTemporal);
        //                        }
        //                        else
        //                        {
        //                            foreach (var item in Out_CodesTemporal)
        //                            {
        //                                Out_Codes.Add(item);
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        foreach (var item in Out_CodesTemporal)
        //                        {
        //                            Out_Codes.Add(item);
        //                        }
        //                    }
        //                }
        //            }
        //            pruebaInternas.Add(new PruebaInterna()
        //            {
        //                id = "00",
        //                method = "GetOut_Codes" + tipoproceso,
        //                quantity = Out_Codes.Count,
        //                message = ErrorMessage,
        //                name = "Out_Codes_cab",
        //                table = false
        //            });
        //        }
        //        else
        //        {
        //            pruebaInternas.Add(new PruebaInterna()
        //            {
        //                id = "00",
        //                method = "GetOut_Codes" + tipoproceso,
        //                quantity = 0,
        //                message = ErrorMessage,
        //                name = "Out_Codes_cab",
        //                table = false
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        pruebaInternas.Add(new PruebaInterna()
        //        {
        //            id = "01",
        //            method = "GetOut_Codes" + tipoproceso,
        //            quantity = 0,
        //            message = ex.Message,
        //            name = "Out_Codes_cab",
        //            table = false
        //        });
        //    }
        //    return Out_Codes;
        //}
        public List<Out_Codes> GetOut_Codes(string storedProcedure, string tipoproceso, bool is_unique_size)
        {
            //Busca y carga marcas nuevas a la tabla Book_In_Codes //Consulta todas las marcas en la tabla Book_In_Codes
            List<Out_Codes> Out_Codes = new List<Out_Codes>();
            List<Out_Codes> Out_CodesTemporal = new List<Out_Codes>();
            Out_Codes Out_CodeTemporal = new Out_Codes();
            List<DimValueOut> dimValues = new List<DimValueOut>();
            List<SizeOut> sizes = new List<SizeOut>();
             
            List<DimValueOut> _codes_dim_temp = new List<DimValueOut>();
            List<DimValueOut> _dimValues_temp = new List<DimValueOut>();
            //DimValueOut _dimValue_temp = new DimValueOut();

            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;
            int Codigoactual = 0;
            bool devuelvedatos = false;

            try
            {
                if (tipoproceso != "-S-E") oracleCommand.Parameters.Add("is_unique_size", OracleDbType.Varchar2).Value = is_unique_size == true ? "1" : "0";
                oracleCommand.Parameters.Add("Out_Codes_C", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, storedProcedure, true))
                { 
                    if (ErrorMessage == "OK")
                    {
                        while (oracleDataReader.Read())
                        {
                            devuelvedatos = true;

                            if (Codigoactual != int.Parse(oracleDataReader["master_code"].ToString().Trim()))
                            {
                                Out_Codes.Add(new Out_Codes()
                                {
                                    master_code = int.Parse(oracleDataReader["master_code"].ToString().Trim()),
                                    parent_sku = oracleDataReader["parent_sku"].ToString().Trim(),
                                    status = int.Parse(oracleDataReader["Cod_Error"].ToString().Trim()),
                                    error = oracleDataReader["Des_error"].ToString().Trim(),
                                    dim_values = new List<DimValueOut>()
                                });
                            }

                            if (tipoproceso != "-S-E")// si es diferente SEND ERROR
                            {
                                if (!dimValues.Any(a => a.master_code == int.Parse(oracleDataReader["master_code"].ToString().Trim()) &&  
                                a.dimension_id == oracleDataReader["dim_values_id"].ToString().Trim()) ) //Valida único para que no se repita
                                {
                                    dimValues.Add(new DimValueOut()
                                    {
                                        master_code = int.Parse(oracleDataReader["master_code"].ToString().Trim()),
                                        dimension_id = oracleDataReader["dim_values_id"].ToString().Trim(),
                                        style_prepack_code = oracleDataReader["dim_values_STYLE_Prepack_code"].ToString().Trim()
                                    });
                                }
                                if (!sizes.Any(a => a.master_code == int.Parse(oracleDataReader["master_code"].ToString().Trim()) &&
                                a.dimCode == oracleDataReader["dim_values_id"].ToString().Trim() &&
                                a.sku == oracleDataReader["dim_values_sizes_sku"].ToString().Trim()) ) //Valida único para que no se repita
                                {
                                    sizes.Add(new SizeOut()
                                    {
                                        master_code = int.Parse(oracleDataReader["master_code"].ToString().Trim()),
                                        dimCode = oracleDataReader["dim_values_id"].ToString().Trim(),
                                        size = oracleDataReader["dim_values_sizes_size"].ToString().Trim(),
                                        sku = oracleDataReader["dim_values_sizes_sku"].ToString().Trim(),
                                        desc = oracleDataReader["dim_values_sizes_desc"].ToString().Trim(),
                                        ean_upc = oracleDataReader["dim_values_ean_upc"].ToString().Trim()

                                    });
                                } 
                               
                            }

                            Codigoactual = int.Parse(oracleDataReader["master_code"].ToString().Trim());
                        }
                        if (devuelvedatos)
                        { 
                            for (int i = 0; i < Out_Codes.Count; i++)
                            { 
                                _dimValues_temp = new List<DimValueOut>();
                                if (dimValues.Any(a=>a.master_code == Out_Codes[i].master_code))// si es S-E no ingresa
                                {
                                    _codes_dim_temp = dimValues.Where(a => a.master_code == Out_Codes[i].master_code).ToList();
                                    foreach(var dim in _codes_dim_temp)
                                    {
                                        dim.sizes = new List<SizeOut>();
                                        if(sizes.Any(a => a.master_code == dim.master_code && a.dimCode == dim.dimension_id))
                                        {
                                            dim.sizes = sizes.Where(a => a.master_code == dim.master_code && a.dimCode == dim.dimension_id).ToList();
                                        }
                                         
                                        _dimValues_temp.Add(dim); 
                                    } 
                                }
                                Out_Codes[i].dim_values = _dimValues_temp;
                            }
                        }
                    } 

                   
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetOut_Codes" + tipoproceso,
                        quantity = Out_Codes.Count,
                        message = ErrorMessage,
                        name = "Out_Codes_cab",
                        table = false
                    });
                }
                else
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetOut_Codes" + tipoproceso,
                        quantity = 0,
                        message = ErrorMessage,
                        name = "Out_Codes_cab",
                        table = false
                    });
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    id = "01",
                    method = "GetOut_Codes" + tipoproceso,
                    quantity = 0,
                    message = ex.Message,
                    name = "Out_Codes_cab",
                    table = false
                });
            }
            return Out_Codes;
        }

        public PruebaInternaDTO DeleteBbook_In_Codes()
        {
            return new PruebaInternaDTO()
            {
                data = new List<PruebaInterna>()
                {
                    _commonRepository.DeleteTable("bbook_prd_master"),
                    _commonRepository.DeleteTable("bbook_prd_sizes"),
                    _commonRepository.DeleteTable("bbook_prd_colors"),
                    _commonRepository.DeleteTable("bbook_prd_attrs")
                }
            };

        }

        public async Task<DTO<Out_Codes>.Response> GetResponseAndUpdateTable(DTOUnitario<Out_Codes>.Request Out_CodesRequest, string tipo)
        {
            HttpResponseMessage res = new HttpResponseMessage();
            DTO<Out_Codes>.Response out_codesResponse = new DTO<Out_Codes>.Response();
            List<string> codigos = new List<string>();
            ExceptionalErrorDTO<In_Codes> exceptionalError;

            try
            {
                res = await _commonRepository.ApiBbook(JsonConvert.SerializeObject(Out_CodesRequest), tipo, "in-codes");
                if (res.Content != null)
                {
                    if (res.StatusCode == HttpStatusCode.OK || res.StatusCode == HttpStatusCode.Conflict)
                    {
                        try
                        {
                            out_codesResponse = JsonConvert.DeserializeObject<DTO<Out_Codes>.Response>(res.Content.ReadAsStringAsync().Result);
                            if (res.StatusCode == HttpStatusCode.OK)
                            {
                                foreach (var item in _commonRepository.UpdateDateTableIn_Codes("prd_master", "A", Out_CodesRequest.data.master_code.ToString()))
                                {
                                    pruebaInternas.Add(item);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                exceptionalError = JsonConvert.DeserializeObject<ExceptionalErrorDTO<In_Codes>>(res.Content.ReadAsStringAsync().Result);
                                return new DTO<Out_Codes>.Response()
                                {
                                    internalCode = exceptionalError.internalCode,
                                    message = exceptionalError.message,
                                    statusCode = exceptionalError.statusCode,
                                    status = exceptionalError.status
                                };
                            }
                            catch
                            {
                                out_codesResponse = new DTO<Out_Codes>.Response()
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
                            out_codesResponse = new DTO<Out_Codes>.Response()
                            {
                                internalCode = res.StatusCode.ToString(),
                                message = res.Content.ReadAsStringAsync().Result,
                                statusCode = 401,
                                status = "error"
                            };
                        }
                        else if (res.StatusCode == HttpStatusCode.RequestEntityTooLarge)
                        {
                            out_codesResponse = new DTO<Out_Codes>.Response()
                            {
                                internalCode = res.StatusCode.ToString(),
                                message = res.Content.ReadAsStringAsync().Result,
                                statusCode = 413,
                                status = "error"
                            };
                        }
                    }
                    out_codesResponse.bulkdata = res.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    out_codesResponse = new DTO<Out_Codes>.Response()
                    {
                        internalCode = "500",
                        message = "Endpoint in-codes no encontrado",
                        statusCode = 413,
                        status = "error",
                        bulkdata = JsonConvert.SerializeObject(res)
                    };
                }
            }
            catch (Exception ex)
            {
                out_codesResponse = new DTO<Out_Codes>.Response()
                {
                    internalCode = "500",
                    message = ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.Source,
                    statusCode = 413,
                    status = "error"
                };
            }

            return out_codesResponse;
        }

        #endregion
    }
}
