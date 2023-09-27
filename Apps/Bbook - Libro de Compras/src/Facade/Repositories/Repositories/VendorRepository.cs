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
using System.Text;
using System.Threading.Tasks;
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Repositories.Repositories
{
    public class VendorRepository:IVendorRepository
    {
        string ErrorMessage;
        private readonly IDBOracleRepository _iDBOracleRepository; //Interface de Conexion a Oracle
        private readonly ICommonRepository _commonRepository; //Interface de metodos comunes
        private List<PruebaInterna> pruebaInternas;
        public VendorRepository(IDBOracleRepository iDBOracleRepository,ICommonRepository commonRepository)
        {
            _iDBOracleRepository = iDBOracleRepository;
            _commonRepository = commonRepository;
        }
        #region Metodos Principales
        public DTO<Vendor>.Request GetAllVendors(bool subproceso)
        {
            pruebaInternas = new List<PruebaInterna>();
            try
            {
                return  new DTO<Vendor>.Request()
                {
                    data = GetVendors("sp_vendor_read","-R"),
                    MessageError = pruebaInternas[0].message
                };
            }
            catch(Exception ex)
            {
                return new DTO<Vendor>.Request()
                {
                    MessageError = ex.Message
                };
            }
            finally
            {
                if(!subproceso)_iDBOracleRepository.Dispose();
            }
        }
        public async Task<DTO<Vendor>> LoadDataVendor()
        {
            List<DTO<Vendor>.Request> vendorRequests = new List<DTO<Vendor>.Request>();
            List<DTO<Vendor>.Response> vendorResponses = new List<DTO<Vendor>.Response>();
            DTO<Vendor>.Request vendorRequest = new DTO<Vendor>.Request();
            pruebaInternas = new List<PruebaInterna>();
            try
            {
                vendorRequest = GetAllVendors(true);
                if(vendorRequest.data.Count == 0) vendorRequest.data = GetVendors("sp_vendor_load", "-L");
                else
                {
                    vendorRequest.data = GetVendors("sp_vendor_create","-A");
                }
                if (vendorRequest.data.Count != 0)
                {
                    //HACER PROCESO HACIA EL BBOOK
                    vendorRequests.Add(new DTO<Vendor>.Request() { data = vendorRequest.data });
                    vendorResponses.Add(await GetResponseAndUpdateTable(vendorRequest,"A"));
                }
                vendorRequest.data = GetVendors("sp_vendor_update","-C");
                if (vendorRequest.data.Count != 0)
                {
                    //HACER PROCESO HACIA EL BBOOK
                    vendorRequests.Add(new DTO<Vendor>.Request() { data = vendorRequest.data });
                    vendorResponses.Add(await GetResponseAndUpdateTable(vendorRequest, "C"));
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    id = "01",
                    method = "LoadDataVendor",
                    quantity = 0,
                    message = ex.Message,
                    table = false,
                    name = "Vendor"
                });
            }
            finally
            {
                _commonRepository.Bbook_history("Bbook_Vendor", JsonConvert.SerializeObject(vendorRequests),
                    JsonConvert.SerializeObject(vendorResponses), JsonConvert.SerializeObject(pruebaInternas));
                _iDBOracleRepository.Dispose();
            }
            return new DTO<Vendor>()
            {
                requests = vendorRequests,
                responses = vendorResponses,
                pruebaInterna = new PruebaInternaDTO() { data = pruebaInternas }
            };
        }
        #endregion
        #region Metodos Extras
        public List<Vendor> GetVendors(string VendorProcedure,string tipoproceso)
        {
            //Busca y carga marcas nuevas a la tabla Book_Vendor //Consulta todas las marcas en la tabla Book_Vendor
            List<Vendor> Vendors = new List<Vendor>();
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracledataReader = null;

            try
            {
                oracleCommand.Parameters.Add("Vendors_C", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                if (_iDBOracleRepository.EjecutaSPBbook(ref oracledataReader, ref oracleCommand,ref ErrorMessage, VendorProcedure, true))
                {
                    if(ErrorMessage == "OK")
                    {
                        int plazo = 0;
                        int area_code = 0;
                        Vendor vendor = new Vendor();

                        while (oracledataReader.Read())
                        {
                            if (oracledataReader["effective_days"].ToString().Trim() != "") plazo = int.Parse(oracledataReader["effective_days"].ToString().Trim());
                            if (oracledataReader["area_code"].ToString().Trim() != "") area_code = int.Parse(oracledataReader["area_code"].ToString().Trim());
                            Vendors.Add(new Vendor()
                            {
                                vendor_id = oracledataReader["vendor_id"].ToString().Trim(),
                                vendor_name = oracledataReader["vendor_name"].ToString().Trim(),
                                inactive = Convert.ToBoolean(int.Parse(oracledataReader["inactive"].ToString().Trim())),
                                origin = oracledataReader["origin"].ToString().Trim(),
                                country = oracledataReader["country"].ToString().Trim(),
                                address_1 = oracledataReader["address_1"].ToString().Trim(),
                                address_2 = oracledataReader["address_2"].ToString().Trim(),
                                city = oracledataReader["city"].ToString().Trim(),
                                area_code = area_code,
                                phone = oracledataReader["phone"].ToString().Trim(),
                                email = oracledataReader["email"].ToString().Trim(),
                                contact_name = oracledataReader["contact_name"].ToString().Trim(),
                                payment_terms = oracledataReader["payment_terms"].ToString().Trim(),
                                effective_days = plazo,
                                currency = oracledataReader["currency"].ToString().Trim()
                            });
                        }
                    }
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetVendors"+ tipoproceso,
                        quantity = Vendors.Count,
                        message = ErrorMessage,
                        name = "Vendor",
                        table = false
                    });
                }
                else
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetVendors"+ tipoproceso,
                        quantity = 0,
                        message = ErrorMessage,
                        name = "Vendor",
                        table = false
                    });
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInternaDTO.PruebaInterna()
                {
                    id = "01",
                    method = "GetVendors"+ tipoproceso,
                    quantity = 0,
                    message = ex.Message,
                    name = "Vendor",
                    table = false
                });
            }
            return Vendors;
        }
        public async Task<DTO<Vendor>.Response> GetResponseAndUpdateTable(DTO<Vendor>.Request vendorRequest, string tipo)
        {
            HttpResponseMessage res;
            DTO<Vendor>.Response vendorResponse = new DTO<Vendor>.Response();
            List<string> codigos = new List<string>();
            ExceptionalErrorDTO<Vendor> exceptionalError;

            res = await _commonRepository.ApiBbook(JsonConvert.SerializeObject(vendorRequest), tipo, "vendors");
            if (res.StatusCode == HttpStatusCode.OK || res.StatusCode == HttpStatusCode.Conflict)
            {
                try
                {
                    vendorResponse = JsonConvert.DeserializeObject<DTO<Vendor>.Response>(res.Content.ReadAsStringAsync().Result);
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        codigos = (from c in vendorRequest.data select c.vendor_id).ToList();
                        foreach (var item in _commonRepository.UpdateDateTable("vendor", tipo, codigos))
                        {
                            pruebaInternas.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        exceptionalError = JsonConvert.DeserializeObject<ExceptionalErrorDTO<Vendor>>(res.Content.ReadAsStringAsync().Result);
                        return new DTO<Vendor>.Response()
                        {
                            internalCode = exceptionalError.internalCode,
                            message = exceptionalError.message,
                            statusCode = exceptionalError.statusCode,
                            status = exceptionalError.status,
                        };
                    }
                    catch
                    {
                        vendorResponse = new DTO<Vendor>.Response()
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
                    vendorResponse = new DTO<Vendor>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 401,
                        status = "error"
                    };
                }
                else if (res.StatusCode == HttpStatusCode.RequestEntityTooLarge)
                {
                    vendorResponse = new DTO<Vendor>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 413,
                        status = "error"
                    };
                }
                else if (res.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    vendorResponse = new DTO<Vendor>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 504,
                        status = "error"
                    };
                }
            }
            return vendorResponse;
        }
        public PruebaInterna DeleteBbook_Vendor()
        {
            return _commonRepository.DeleteTable("Bbook_Vendor");
        }
        #endregion
    }
}
