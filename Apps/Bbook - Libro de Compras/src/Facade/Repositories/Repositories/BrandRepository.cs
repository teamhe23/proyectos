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
    public class BrandRepository : IBrandRepository
    {
        string ErrorMessage;
        private readonly IDBOracleRepository _iDBOracleRepository; //Interface de Conexion a Oracle
        private readonly ICommonRepository _commonRepository; //Interface de metodos comunes
        private List<PruebaInterna> pruebaInternas;
        public BrandRepository(IDBOracleRepository iDBOracleRepository, ICommonRepository commonRepository)
        {
            _iDBOracleRepository = iDBOracleRepository;
            _commonRepository = commonRepository;
        }
        #region Metodos Principales
        public DTO<Brand>.Request GetAllBrands()
        {
            pruebaInternas = new List<PruebaInterna>();
            try
            {
                return new DTO<Brand>.Request()
                {
                    data = GetBrands("sp_brand_read", ""),
                    MessageError = pruebaInternas[0].message
                };
            }
            catch (Exception ex)
            {
                return new DTO<Brand>.Request()
                {
                    MessageError = ex.Message
                };
            }
            finally
            {
                _iDBOracleRepository.Dispose();
            }
        }
        public async Task<DTO<Brand>> LoadDataBrand()
        {
            List<DTO<Brand>.Request> brandRequests = new List<DTO<Brand>.Request>();
            List<DTO<Brand>.Response> brandResponses = new List<DTO<Brand>.Response>();
            DTO<Brand>.Request brandRequest = new DTO<Brand>.Request();
            pruebaInternas = new List<PruebaInterna>();
            try
            {
                brandRequest.data = GetBrands("sp_brand_create", "-A");
                if (brandRequest.data.Count != 0)
                {
                    //HACER PROCESO POST HACIA EL BBOOK
                    brandRequests.Add(new DTO<Brand>.Request() { data = brandRequest.data });
                    brandResponses.Add(await GetResponseAndUpdateTable(brandRequest, "A"));
                }
                brandRequest.data = GetBrands("sp_brand_update", "-C");
                if (brandRequest.data.Count != 0)
                {
                    //HACER PROCESO PUT HACIA EL BBOOK
                    brandRequests.Add(new DTO<Brand>.Request() { data = brandRequest.data });
                    brandResponses.Add(await GetResponseAndUpdateTable(brandRequest, "C"));
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    id = "01",
                    method = "LoadDataBrand",
                    quantity = 0,
                    message = ex.Message,
                    table = false,
                    name = "Brand"
                });
            }
            finally
            {
                _commonRepository.Bbook_history("Bbook_Brand", JsonConvert.SerializeObject(brandRequests),
                    JsonConvert.SerializeObject(brandResponses), JsonConvert.SerializeObject(pruebaInternas));
                _iDBOracleRepository.Dispose();
            }
            return new DTO<Brand>()
            {
                requests = brandRequests,
                responses = brandResponses,
                pruebaInterna = new PruebaInternaDTO() { data = pruebaInternas }
            };
        }
        #endregion
        #region Metodos Extras
        public List<Brand> GetBrands(string storedProcedure, string tipoproceso)
        {
            //Busca y carga marcas nuevas a la tabla Book_Brand //Consulta todas las marcas en la tabla Book_Brand
            List<Brand> Brands = new List<Brand>();
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;

            try
            {
                oracleCommand.Parameters.Add("Brands_C", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, storedProcedure, true))
                {
                    if (ErrorMessage == "OK")
                    {
                        while (oracleDataReader.Read())
                        {
                            Brands.Add(new Brand()
                            {
                                id = oracleDataReader["brand_id"].ToString().Trim(),
                                name = oracleDataReader["brand_name"].ToString().Trim()
                            });
                        }
                    }
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetBrands" + tipoproceso,
                        quantity = Brands.Count,
                        message = ErrorMessage,
                        name = "Brand",
                        table = false
                    });
                }
                else
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetBrands" + tipoproceso,
                        quantity = 0,
                        message = ErrorMessage,
                        name = "Brand",
                        table = false
                    });
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    id = "01",
                    method = "GetBrands" + tipoproceso,
                    quantity = 0,
                    message = ex.Message,
                    name = "Brand",
                    table = false
                });
            }
            return Brands;
        }
        public async Task<DTO<Brand>.Response> GetResponseAndUpdateTable(DTO<Brand>.Request brandRequest, string tipo)
        {
            HttpResponseMessage res = new HttpResponseMessage();
            DTO<Brand>.Response brandResponse = new DTO<Brand>.Response();
            List<string> codigos = new List<string>();
            ExceptionalErrorDTO<Brand> exceptionalError;
            List<ErrorDTO<Brand>> l_marca_errores = new List<ErrorDTO<Brand>>();
            List<Brand> l_marcas_patch = new List<Brand>();
             DTO<Brand>.Response brandResponse_exito = null;
            ///////////////
            ///
            //--1.Enviar con intentos
            int nro_intentos = 5;
            for (int contador = 0; contador < nro_intentos; contador++)
            {
                brandResponse = null;
                res = await _commonRepository.ApiBbook(JsonConvert.SerializeObject(brandRequest), tipo, "brands");
                switch (res.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.Conflict:
                        try
                        {
                            brandResponse = JsonConvert.DeserializeObject<DTO<Brand>.Response>(res.Content.ReadAsStringAsync().Result);
                            if (res.StatusCode == HttpStatusCode.OK)
                            {
                                codigos = (from c in brandRequest.data select c.id).ToList();
                                foreach (var item in _commonRepository.UpdateDateTable("brand", tipo, codigos))
                                {
                                    pruebaInternas.Add(item);
                                }
                                brandResponse_exito = brandResponse;
                                contador = nro_intentos;// para salir del bucle
                            }
                            else // cuando es HttpStatusCode.Conflict
                            {
                                if (brandResponse.errors != null)
                                {
                                    if (brandResponse.errors.Count > 0)
                                    {
                                        ErrorDTO<Brand> error1 = brandResponse.errors.FirstOrDefault();
                                        Brand marca_error = brandRequest.data.Where(a => a.id == error1.record.id).ToList().FirstOrDefault();
                                        //l_marca_errores.Add(error1);
                                        l_marca_errores.Add(new ErrorDTO<Brand>()
                                        {
                                            code = error1.code,
                                            record = marca_error
                                        });
                                        brandRequest.data.Remove(marca_error);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                exceptionalError = JsonConvert.DeserializeObject<ExceptionalErrorDTO<Brand>>(res.Content.ReadAsStringAsync().Result);
                                return new DTO<Brand>.Response()
                                {
                                    internalCode = exceptionalError.internalCode,
                                    message = exceptionalError.message,
                                    statusCode = exceptionalError.statusCode,
                                    status = exceptionalError.status
                                };
                            }
                            catch
                            {
                                brandResponse = new DTO<Brand>.Response()
                                {
                                    internalCode = "JsonError",
                                    message = "Error al deserializar Json" + "\n" + ex.Message,
                                    statusCode = 00,
                                    status = "error"
                                };
                            }
                        }
                        break;
                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.RequestEntityTooLarge:
                    case HttpStatusCode.GatewayTimeout:
                        brandResponse = new DTO<Brand>.Response()
                        {
                            internalCode = res.StatusCode.ToString(),
                            message = res.Content.ReadAsStringAsync().Result,
                            statusCode = int.Parse(res.StatusCode.ToString()),
                            status = "error"
                        };
                        break;
                }
            }

            //--2.-validar errores
            foreach (var item in l_marca_errores)
            {
                if (item.code == "IF500")
                {
                    l_marcas_patch.Add(item.record);
                }
            }
            //--3.-volver a enviar marcas q tuvieron errores
            if (l_marcas_patch.Count > 0)
            {
                DTO<Brand>.Request brandRequest_patch = new DTO<Brand>.Request();
                brandRequest_patch.data = l_marcas_patch;

                //res = await _commonRepository.ApiBbook(JsonConvert.SerializeObject(brandRequest_patch), "PA", "brands"); //<12/05/2022>
                res = await _commonRepository.ApiBbook(JsonConvert.SerializeObject(brandRequest_patch), tipo, "brands"); 
                switch (res.StatusCode)
                {
                    case HttpStatusCode.OK:
                        //case HttpStatusCode.Conflict:
                        try
                        {
                            brandResponse = JsonConvert.DeserializeObject<DTO<Brand>.Response>(res.Content.ReadAsStringAsync().Result);
                            if (res.StatusCode == HttpStatusCode.OK)
                            {
                                codigos = (from c in brandRequest_patch.data select c.id).ToList();
                                foreach (var item in _commonRepository.UpdateDateTable("brand", tipo, codigos))
                                {
                                    pruebaInternas.Add(item);
                                }                                
                            }
                            else // cuando es HttpStatusCode.Conflict
                            {
                            }
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                exceptionalError = JsonConvert.DeserializeObject<ExceptionalErrorDTO<Brand>>(res.Content.ReadAsStringAsync().Result);
                                return new DTO<Brand>.Response()
                                {
                                    internalCode = exceptionalError.internalCode,
                                    message = exceptionalError.message,
                                    statusCode = exceptionalError.statusCode,
                                    status = exceptionalError.status
                                };
                            }
                            catch
                            {
                                brandResponse = new DTO<Brand>.Response()
                                {
                                    internalCode = "JsonError",
                                    message = "Error al deserializar Json" + "\n" + ex.Message,
                                    statusCode = 00,
                                    status = "error"
                                };
                            }
                        }
                        break;
                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.RequestEntityTooLarge:
                    case HttpStatusCode.GatewayTimeout:
                        brandResponse = new DTO<Brand>.Response()
                        {
                            internalCode = res.StatusCode.ToString(),
                            message = res.Content.ReadAsStringAsync().Result,
                            statusCode = int.Parse(res.StatusCode.ToString()),
                            status = "error"
                        };
                        break;
                } 
            }

            if(brandResponse_exito != null)
            {
                brandResponse = brandResponse_exito;
            }
            ////////
            return brandResponse;
        }

        public async Task<DTO<Brand>.Response> GetResponseAndUpdateTable_ANTES(DTO<Brand>.Request brandRequest, string tipo)
        {
            HttpResponseMessage res = new HttpResponseMessage();
            DTO<Brand>.Response brandResponse = new DTO<Brand>.Response();
            List<string> codigos = new List<string>();
            ExceptionalErrorDTO<Brand> exceptionalError;

            res = await _commonRepository.ApiBbook(JsonConvert.SerializeObject(brandRequest), tipo, "brands");
            if (res.StatusCode == HttpStatusCode.OK || res.StatusCode == HttpStatusCode.Conflict)
            {
                try
                {
                    brandResponse = JsonConvert.DeserializeObject<DTO<Brand>.Response>(res.Content.ReadAsStringAsync().Result);
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        codigos = (from c in brandRequest.data select c.id).ToList();
                        foreach (var item in _commonRepository.UpdateDateTable("brand", tipo, codigos))
                        {
                            pruebaInternas.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        exceptionalError = JsonConvert.DeserializeObject<ExceptionalErrorDTO<Brand>>(res.Content.ReadAsStringAsync().Result);
                        return new DTO<Brand>.Response()
                        {
                            internalCode = exceptionalError.internalCode,
                            message = exceptionalError.message,
                            statusCode = exceptionalError.statusCode,
                            status = exceptionalError.status
                        };
                    }
                    catch
                    {
                        brandResponse = new DTO<Brand>.Response()
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
                    brandResponse = new DTO<Brand>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 401,
                        status = "error"
                    };
                }
                else if (res.StatusCode == HttpStatusCode.RequestEntityTooLarge)
                {
                    brandResponse = new DTO<Brand>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 413,
                        status = "error"
                    };
                }
                else if (res.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    brandResponse = new DTO<Brand>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 504,
                        status = "error"
                    };
                }
            }
            return brandResponse;
        }
        public PruebaInterna DeleteBbook_Brand()
        {
            return _commonRepository.DeleteTable("Bbook_brand");
        }
        #endregion
    }
}
