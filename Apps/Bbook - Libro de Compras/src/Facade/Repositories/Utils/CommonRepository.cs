using Facade.Models.In_Asn;
using Facade.Models.In_Comex;
using IntegracionBbook.Api.Models;
using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Data.Interfaces;
using IntegracionBbook.Models;
using IntegracionBbook.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using ModernHttpClient;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Polly;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static IntegracionBbook.Models.PruebaInternaDTO;
using System.Text.RegularExpressions;
using System.Linq;
using System;

namespace IntegracionBbook.Repositories.Repositories
{
    public class CommonRepository : ICommonRepository
    {
        string ErrorMessage;
        private readonly IDBOracleRepository _iDBOracleRepository; //Interface de Conexion a Oracle
        private readonly IConfiguration _Config;

        public CommonRepository(IDBOracleRepository iDBOracleRepository, IConfiguration Config)
        {
            _iDBOracleRepository = iDBOracleRepository;
            _Config = Config;
        }

        public PruebaInterna DeleteTable(string nameTable)
        {
            int numfilas = 0;
            try
            {
                _iDBOracleRepository.EjecutaSQL("delete from " + nameTable, ref ErrorMessage, ref numfilas);
                return new PruebaInterna()
                {
                    table = true,
                    method = "DeleteTable " + nameTable,
                    name = nameTable,
                    quantity = numfilas,
                    message = ErrorMessage
                };
            }
            catch (Exception ex)
            {
                return new PruebaInterna()
                {
                    table = true,
                    name = nameTable,
                    quantity = numfilas,
                    message = ex.Message
                };
            }
        }

        public List<PruebaInterna> DeleteTableIn_po(string nameTable)
        {
            List<PruebaInterna> pruebaInternas = new List<PruebaInterna>();
            int numfilas = 0;
            try
            {
                _iDBOracleRepository.EjecutaSQL("delete from " + nameTable + "_cab", ref ErrorMessage, ref numfilas);
                pruebaInternas.Add(new PruebaInterna()
                {
                    table = true,
                    method = "DeleteTable " + nameTable + "_cab",
                    name = nameTable,
                    quantity = numfilas,
                    message = ErrorMessage
                });
                _iDBOracleRepository.EjecutaSQL("delete from " + nameTable + "_det", ref ErrorMessage, ref numfilas);
                pruebaInternas.Add(new PruebaInterna()
                {
                    table = true,
                    method = "DeleteTable " + nameTable + "_det",
                    name = nameTable,
                    quantity = numfilas,
                    message = ErrorMessage
                });
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    table = true,
                    name = nameTable,
                    quantity = numfilas,
                    message = ex.Message
                });
            }
            return pruebaInternas;
        }

        public List<PruebaInterna> DeleteTableIn_Codes(string nameTable)
        {
            List<PruebaInterna> pruebaInternas = new List<PruebaInterna>();
            int numfilas = 0;
            try
            {
                _iDBOracleRepository.EjecutaSQL("delete from " + nameTable + "_master", ref ErrorMessage, ref numfilas);
                pruebaInternas.Add(new PruebaInterna()
                {
                    table = true,
                    method = "DeleteTable " + nameTable + "_master",
                    name = nameTable,
                    quantity = numfilas,
                    message = ErrorMessage
                });

                _iDBOracleRepository.EjecutaSQL("delete from " + nameTable + "_sizes", ref ErrorMessage, ref numfilas);
                pruebaInternas.Add(new PruebaInterna()
                {
                    table = true,
                    method = "DeleteTable " + nameTable + "_sizes",
                    name = nameTable,
                    quantity = numfilas,
                    message = ErrorMessage
                });

                _iDBOracleRepository.EjecutaSQL("delete from " + nameTable + "_colors", ref ErrorMessage, ref numfilas);
                pruebaInternas.Add(new PruebaInterna()
                {
                    table = true,
                    method = "DeleteTable " + nameTable + "_colors",
                    name = nameTable,
                    quantity = numfilas,
                    message = ErrorMessage
                });

                _iDBOracleRepository.EjecutaSQL("delete from " + nameTable + "_attrs", ref ErrorMessage, ref numfilas);
                pruebaInternas.Add(new PruebaInterna()
                {
                    table = true,
                    method = "DeleteTable " + nameTable + "_attrs",
                    name = nameTable,
                    quantity = numfilas,
                    message = ErrorMessage
                });
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    table = true,
                    name = nameTable,
                    quantity = numfilas,
                    message = ex.Message
                });
            }
            return pruebaInternas;
        }

        public async Task<HttpResponseMessage> ApiBbook(string json, string tipo, string endpoint)
        {
            string token = _Config["Bbook:token"];
            string urlBbook = _Config["Bbook:url"];
            HttpClient client = new HttpClient(new NativeMessageHandler());
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("x-bbook-token", token);

            if (endpoint.Contains("in-")) urlBbook = urlBbook.Replace("/v1/", "/v2/");

            HttpResponseMessage res = new HttpResponseMessage();
            CancellationToken cancellationToken = CancellationToken.None;
            var policyRetry = Policy.Handle<HttpRequestException>().Retry(6);
            var policyTimeout = Policy.Timeout(60);
            var policyWrap = Policy.Wrap(policyRetry, policyTimeout);

            try
            {
                //
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                //res = await policyWrap.Execute(async ct => await client.PostAsync("https://bbookprobador20210428094848.azurewebsites.net/api/Bbook_Receptor/" + endpoint, data), cancellationToken);
                //tipo: A = Add C = Change M = Move S = Send R = Read
                if (tipo == "A" || tipo == "S")
                    res = await policyWrap.Execute(async ct => await client.PostAsync(urlBbook + endpoint, data), cancellationToken);
                else if (tipo == "C")
                    res = await policyWrap.Execute(async ct => await client.PutAsync(urlBbook + endpoint, data), cancellationToken);
                else
                    res = await policyWrap.Execute(async ct => await client.PatchAsync(urlBbook + endpoint, data), cancellationToken);


                /////////
                //tipo: A = Add C = Change M = Move S = Send R = Read
                switch (tipo)
                {
                    case "A":
                    case "S":
                        res = await policyWrap.Execute(async ct => await client.PostAsync(urlBbook + endpoint, data), cancellationToken);
                        break;
                    case "C":
                        res = await policyWrap.Execute(async ct => await client.PutAsync(urlBbook + endpoint, data), cancellationToken);
                        break;
                    case "PA":
                        res = await policyWrap.Execute(async ct => await client.PatchAsync(urlBbook + endpoint, data), cancellationToken);
                        break;
                    default:
                        res = await policyWrap.Execute(async ct => await client.PatchAsync(urlBbook + endpoint, data), cancellationToken);
                        break;
                }

                res.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                res.ReasonPhrase = ex.Message;
                Console.WriteLine("Error " + res + " Error " +
                ex.ToString());
            }

            Console.WriteLine("Response: {0}", "");
            return res;
        }

        public List<PruebaInterna> UpdateDateTable(string nameTable, string tipo, List<string> codigos)
        {
            List<PruebaInterna> pruebaInternas = new List<PruebaInterna>();
            foreach (var item in codigos)
            {
                int numfilas = 0;
                //tipo: A=Add C=Change M=Move S=Send R=Read
                string scriptBase = "Update TIENDAS_ADM.Bbook_" + nameTable + " set date_envio = trunc(Sysdate), " + nameTable + "_type_change = 'S'" +
                    " Where " + nameTable + "_type_change = '" + tipo.ToString() + "' and " + nameTable + "_id = '" + item.Replace("'", "''") + "' and date_envio is null";
                try
                {
                    _iDBOracleRepository.EjecutaSQL(scriptBase, ref ErrorMessage, ref numfilas);
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        table = true,
                        name = nameTable,
                        id = item,
                        quantity = numfilas,
                        method = "UpdateDateTable " + nameTable + "-" + tipo,
                        message = ErrorMessage
                    }
                    );
                }
                catch (Exception ex)
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        table = true,
                        name = nameTable,
                        id = item,
                        quantity = numfilas,
                        method = "UpdateDateTable " + nameTable + "-" + tipo == "A" ? "Post" : "Put",
                        message = ex.Message
                    });
                }
            }

            return pruebaInternas;
        }

        public List<PruebaInterna> UpdateDateTableProduct(string nameTable, string tipo, List<string> codigos)
        {
            List<PruebaInterna> pruebaInternas = new List<PruebaInterna>();
            foreach (var item in codigos)
            {
                int numfilas = 0;
                //tipo: A=Add C=Change M=Move S=Send R=Read
                string scriptBase = "Update TIENDAS_ADM.Bbook_" + nameTable + " set date_envio = trunc(Sysdate), " + nameTable + "_type_change = 'S'" +
                    " Where " + nameTable + "_type_change = '" + tipo.ToString() +
                    //"' and sizes_sku = '" + item.Replace("'", "''") + 
                    "' and parent_sku = '" + item.Replace("'", "''") +
                    "' and date_envio is null";
                try
                {
                    _iDBOracleRepository.EjecutaSQL(scriptBase, ref ErrorMessage, ref numfilas);
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        table = true,
                        name = nameTable,
                        id = item,
                        quantity = numfilas,
                        method = "UpdateDateTable " + nameTable + "-" + tipo,
                        message = ErrorMessage
                    }
                    );
                }
                catch (Exception ex)
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        table = true,
                        name = nameTable,
                        id = item,
                        quantity = numfilas,
                        method = "UpdateDateTable " + nameTable + "-" + tipo == "A" ? "Post" : "Put",
                        message = ex.Message
                    });
                }
            }

            return pruebaInternas;
        }

        public List<PruebaInterna> UpdateDateTableSize(string nameTable, string tipo, List<string> codigos)
        {
            List<PruebaInterna> pruebaInternas = new List<PruebaInterna>();
            foreach (var item in codigos)
            {
                int numfilas = 0;
                //tipo: A=Add C=Change M=Move S=Send R=Read 
                string scriptBase = "Update TIENDAS_ADM.Bbook_" + nameTable + " set date_envio = trunc(Sysdate), " + nameTable + "_type_change = 'S'" +
                    " Where " + nameTable + "_type_change = '" + tipo.ToString() + "' and type_id = '" + item.Replace("'", "''") + "' and date_envio is null";
                try
                {
                    _iDBOracleRepository.EjecutaSQL(scriptBase, ref ErrorMessage, ref numfilas);
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        table = true,
                        name = nameTable,
                        id = item,
                        quantity = numfilas,
                        method = "UpdateDateTable " + nameTable + "-" + tipo,
                        message = ErrorMessage
                    }
                    );
                }
                catch (Exception ex)
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        table = true,
                        name = nameTable,
                        id = item,
                        quantity = numfilas,
                        method = "UpdateDateTable " + nameTable + "-" + tipo == "A" ? "Post" : "Put",
                        message = ex.Message
                    });
                }
            }

            return pruebaInternas;
        }

        public List<PruebaInterna> UpdateDateTableMaster_po(string nameTable, string tipo, List<string> codigos)
        {
            List<PruebaInterna> pruebaInternas = new List<PruebaInterna>();
            foreach (var item in codigos)
            {
                int numfilas = 0;
                //tipo: A=Add C=Change M=Move S=Send R=Read 
                string scriptBase = "Update TIENDAS_ADM.Bbook_" + nameTable + " set date_envio = trunc(Sysdate), " + nameTable + "_type_change = 'S'" +
                    " Where " + nameTable + "_type_change = '" + tipo.ToString() + "' and purchase_order = '" + item.Replace("'", "''") + "' and date_envio is null";
                try
                {
                    _iDBOracleRepository.EjecutaSQL(scriptBase, ref ErrorMessage, ref numfilas);
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        table = true,
                        name = nameTable,
                        id = item,
                        quantity = numfilas,
                        method = "UpdateDateTable " + nameTable + "-" + tipo,
                        message = ErrorMessage
                    }
                    );
                }
                catch (Exception ex)
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        table = true,
                        name = nameTable,
                        id = item,
                        quantity = numfilas,
                        method = "UpdateDateTable " + nameTable + "-" + tipo == "A" ? "Post" : "Put",
                        message = ex.Message
                    });
                }
            }

            return pruebaInternas;
        }

        public List<PruebaInterna> UpdateDateTableHierarchy(string nameTable, string tipo, List<string> codigos)
        {
            List<PruebaInterna> pruebaInternas = new List<PruebaInterna>();
            foreach (var item in codigos)
            {
                int numfilas = 0;
                string scriptBase = "";
                //tipo: A=Add C=Change M=Move S=Send R=Read 
                if (item.StartsWith("D"))
                {
                    scriptBase = "Update TIENDAS_ADM.Bbook_" + nameTable + " set date_envio = trunc(Sysdate), " + nameTable + "_type_change = 'S'" +
                    " Where " + nameTable + "_type_change = '" + tipo.ToString() + "' and department_id = '" + item.Replace("'", "''").Trim() + "' and date_envio is null";
                }
                else if (item.StartsWith("L"))
                {
                    scriptBase = "Update TIENDAS_ADM.Bbook_" + nameTable + " set date_envio = trunc(Sysdate), " + nameTable + "_type_change = 'S'" +
                    " Where " + nameTable + "_type_change = '" + tipo.ToString() + "' and line_id = '" + item.Replace("'", "''").Trim() + "' and date_envio is null";
                }
                else
                {
                    scriptBase = "Update TIENDAS_ADM.Bbook_" + nameTable + " set date_envio = trunc(Sysdate), " + nameTable + "_type_change = 'S'" +
                    " Where " + nameTable + "_type_change = '" + tipo.ToString() + "' and division_id = '" + item.Replace("'", "''").Trim() + "' and date_envio is null";
                }
                try
                {
                    _iDBOracleRepository.EjecutaSQL(scriptBase, ref ErrorMessage, ref numfilas);
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        table = true,
                        name = nameTable,
                        id = item,
                        quantity = numfilas,
                        method = "UpdateDateTable " + nameTable + "-" + tipo,
                        message = ErrorMessage
                    }
                    );
                }
                catch (Exception ex)
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        table = true,
                        name = nameTable,
                        id = item,
                        quantity = numfilas,
                        method = "UpdateDateTable " + nameTable + "-" + tipo == "A" ? "Post" : "Put",
                        message = ex.Message
                    });
                }
            }

            return pruebaInternas;
        }

        public List<PruebaInterna> UpdateDateTableIn_po(string nameTable, string tipo, string codigo)
        {
            List<PruebaInterna> pruebaInternas = new List<PruebaInterna>();

            int numfilas = 0;
            //tipo: A=Add C=Change M=Move S=Send R=Read
            string scriptBaseCab = "Update TIENDAS_ADM.Bbook_" + nameTable + "_cab" + " set date_envio = trunc(Sysdate), " + nameTable + "_cab" + "_type_change = 'S' , FEC_ENVIO = sysdate" +
                " Where " + nameTable + "_cab" + "_type_change = '" + tipo.ToString() + "' and " + " in_po_cab_id = '" + codigo.Replace("'", "''") + "' and date_envio is not null";
            string scriptBaseDet = "Update TIENDAS_ADM.Bbook_" + nameTable + "_det" + " set date_envio = trunc(Sysdate), " + nameTable + "_det" + "_type_change = 'S'" +
                " Where " + nameTable + "_det" + "_type_change = '" + tipo.ToString() + "' and " + " in_po_cab_id = '" + codigo.Replace("'", "''") + "' and date_envio is not null";
            try
            {
                _iDBOracleRepository.EjecutaSQL(scriptBaseCab, ref ErrorMessage, ref numfilas);
                pruebaInternas.Add(new PruebaInterna()
                {
                    table = true,
                    name = nameTable,
                    id = codigo,
                    quantity = numfilas,
                    method = "UpdateDateTable " + nameTable + "-cab-" + tipo,
                    message = ErrorMessage
                });
                _iDBOracleRepository.EjecutaSQL(scriptBaseDet, ref ErrorMessage, ref numfilas);
                pruebaInternas.Add(new PruebaInterna()
                {
                    table = true,
                    name = nameTable,
                    id = codigo,
                    quantity = numfilas,
                    method = "UpdateDateTable " + nameTable + "-det-" + tipo,
                    message = ErrorMessage
                });
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    table = true,
                    name = nameTable,
                    id = codigo,
                    quantity = numfilas,
                    method = "UpdateDateTable " + nameTable + "-" + tipo == "A" ? "Post" : "Put",
                    message = ex.Message
                });
            }

            return pruebaInternas;
        }

        public List<PruebaInterna> UpdateDateTableIn_Codes(string nameTable, string tipo, string codigo)//ToDo:Cambiar esto
        {
            List<PruebaInterna> pruebaInternas = new List<PruebaInterna>();

            int numfilas = 0;
            //tipo: A=Add C=Change M=Move S=Send R=Read
            string scriptBase = "Update TIENDAS_ADM.Bbook_" + nameTable + " set date_envio = trunc(Sysdate), " + nameTable + "_type_change = 'S' , fec_envio_date = sysdate " +
                "Where " + nameTable + "_type_change = '" + tipo.ToString() + "' and master_code = '" + codigo.Replace("'", "''") + "' and date_envio is null";
            try
            {
                _iDBOracleRepository.EjecutaSQL(scriptBase, ref ErrorMessage, ref numfilas);
                pruebaInternas.Add(new PruebaInterna()
                {
                    table = true,
                    name = nameTable,
                    id = codigo,
                    quantity = numfilas,
                    method = "UpdateDateTable " + nameTable + tipo,
                    message = ErrorMessage
                });
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    table = true,
                    name = nameTable,
                    id = codigo,
                    quantity = numfilas,
                    method = "UpdateDateTable " + nameTable + tipo == "A" ? "Post" : "Put",
                    message = ex.Message
                });
            }

            return pruebaInternas;
        }

        public List<PruebaInterna> AddDataTableHierarchy(string nameTable, string tipo, IEnumerable<IEnumerable<string>> codigos)
        {
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;
            List<PruebaInterna> pruebaInternas = new List<PruebaInterna>();
            foreach (var item in codigos)
            {
                foreach (var code in item)
                {
                    try
                    {
                        oracleCommand.Parameters.Add("Line_id", code);
                        if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, "sp_hierarchy_insert", false))
                        {
                            pruebaInternas.Add(new PruebaInterna()
                            {
                                table = true,
                                name = nameTable,
                                id = code,
                                quantity = 1,
                                message = ErrorMessage,
                                method = "AddDataTable" + nameTable + tipo
                            });
                        }
                        else
                        {
                            pruebaInternas.Add(new PruebaInterna()
                            {
                                table = true,
                                name = nameTable,
                                id = code,
                                quantity = 0,
                                message = "CONEXION A BASE DE DATOS",
                                method = "AddDataTable" + nameTable + tipo
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        pruebaInternas.Add(new PruebaInterna()
                        {
                            table = true,
                            name = nameTable,
                            id = code,
                            quantity = 0,
                            message = ex.Message,
                            method = "AddDataTable" + nameTable + tipo
                        });
                    }
                }
            }

            return pruebaInternas;
        }

        public List<PruebaInterna> _AddDataTableIn_po(string nameTable, string tipo, DTOUnitario<In_Po>.Request in_PoRequest)
        {
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;
            List<PruebaInterna> pruebaInternas = new List<PruebaInterna>();
            int audit_number = 0;
            string ext_po_num = "";
            int codigo = 0;

            try
            {
                //SP_P_ADD_TPIMPOCC Y TABLA IN_PO_CAB
                oracleCommand.Parameters.Add("audit_number", OracleDbType.Int32).Direction = ParameterDirection.Output;
                oracleCommand.Parameters.Add("in_po_cab_id", OracleDbType.Int32).Direction = ParameterDirection.Output;
                oracleCommand.Parameters.Add("ext_po_num", OracleDbType.Varchar2, size: 50).Direction = ParameterDirection.Output;
                oracleCommand.Parameters.Add("id_document", OracleDbType.Varchar2).Value = in_PoRequest.data.id_document.ToString();
                oracleCommand.Parameters.Add("proforma_invoice", OracleDbType.Varchar2).Value = in_PoRequest.data.proforma_invoice;
                oracleCommand.Parameters.Add("po_type", OracleDbType.Varchar2).Value = in_PoRequest.data.po_type == "I" ? "OC Importado" : "OC Nacional";
                oracleCommand.Parameters.Add("distribution", OracleDbType.Int32).Value = in_PoRequest.data.distribution == "S" ? 2 : 6;
                oracleCommand.Parameters.Add("is_style_prepack", OracleDbType.Int32).Value = in_PoRequest.data.is_style_prepack == true ? 1 : 0;
                oracleCommand.Parameters.Add("vendor_id", OracleDbType.Varchar2).Value = in_PoRequest.data.vendor.id;
                oracleCommand.Parameters.Add("vendor_name", OracleDbType.Varchar2).Value = in_PoRequest.data.vendor.name;
                oracleCommand.Parameters.Add("sucursal", OracleDbType.Int32).Value = in_PoRequest.data.destination.id;
                oracleCommand.Parameters.Add("buyer_id", OracleDbType.Varchar2).Value = in_PoRequest.data.buyer.name;
                oracleCommand.Parameters.Add("buyer_name", OracleDbType.Varchar2).Value = in_PoRequest.data.buyer.name;
                oracleCommand.Parameters.Add("delivery_date", OracleDbType.Varchar2).Value = in_PoRequest.data.dates.delivery;
                oracleCommand.Parameters.Add("cancellation_date", OracleDbType.Varchar2).Value = in_PoRequest.data.dates.cancellation;
                oracleCommand.Parameters.Add("shipment_date", OracleDbType.Varchar2).Value = in_PoRequest.data.dates.shipment;
                oracleCommand.Parameters.Add("reception_date", OracleDbType.Varchar2).Value = in_PoRequest.data.dates.reception;
                oracleCommand.Parameters.Add("import_factor", OracleDbType.Decimal).Value = in_PoRequest.data.import_factor;
                oracleCommand.Parameters.Add("change_type", OracleDbType.Decimal).Value = in_PoRequest.data.change_type;
                oracleCommand.Parameters.Add("port_of_loading_id", OracleDbType.Varchar2).Value = in_PoRequest.data.port_of_loading.id;
                oracleCommand.Parameters.Add("port_of_loading_name", OracleDbType.Varchar2).Value = in_PoRequest.data.port_of_loading.name;
                oracleCommand.Parameters.Add("port_of_discharge_id", OracleDbType.Varchar2).Value = in_PoRequest.data.port_of_discharge.id;
                oracleCommand.Parameters.Add("port_of_discharge_name", OracleDbType.Varchar2).Value = in_PoRequest.data.port_of_discharge.name;
                oracleCommand.Parameters.Add("payment_terms", OracleDbType.Varchar2).Value = in_PoRequest.data.payment_terms.name;
                oracleCommand.Parameters.Add("incoterm", OracleDbType.Varchar2).Value = in_PoRequest.data.incoterm;


                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, "SP_IN_PO_CAB_CREATE", true))
                {
                    if (ErrorMessage == "OK")
                    {
                        int item = 0;
                        audit_number = int.Parse(oracleCommand.Parameters["audit_number"].Value.ToString());
                        codigo = int.Parse(oracleCommand.Parameters["in_po_cab_id"].Value.ToString());
                        ext_po_num = oracleCommand.Parameters["ext_po_num"].Value.ToString();
                        pruebaInternas.Add(new PruebaInterna()
                        {
                            table = true,
                            name = nameTable,
                            id = codigo.ToString(),
                            quantity = 1,
                            message = ErrorMessage,
                            method = "AddDataTable" + nameTable + "_cab" + "-" + tipo
                        });
                        foreach (var product in in_PoRequest.data.products)
                        {
                            string sku = string.Empty;
                            oracleCommand = new OracleCommand();
                            //foreach (var destination in product.destination)
                            //{
                            item++;
                            oracleCommand = new OracleCommand();
                            for (int i = 0; i < (15 - product.sku.Length + 1); i++)
                            {
                                if (i == 0) sku = product.sku;
                                else sku += " ";
                            }
                            oracleCommand.Parameters.Add("audit_number_cab", OracleDbType.Int32).Value = audit_number;
                            oracleCommand.Parameters.Add("in_po_cab_id", OracleDbType.Int32).Value = codigo;
                            oracleCommand.Parameters.Add("line_number", OracleDbType.Int32).Value = item;
                            oracleCommand.Parameters.Add("sku", OracleDbType.Varchar2).Value = sku;
                            oracleCommand.Parameters.Add("org_lvl_number", OracleDbType.Int32).Value = in_PoRequest.data.destination.id;
                            oracleCommand.Parameters.Add("quantity", OracleDbType.Int32).Value = product.total_units;
                            oracleCommand.Parameters.Add("distribution", OracleDbType.Int32).Value = in_PoRequest.data.distribution == "S" ? 2 : 6;
                            oracleCommand.Parameters.Add("id_document", OracleDbType.Varchar2).Value = in_PoRequest.data.id_document.ToString();
                            oracleCommand.Parameters.Add("ext_po_num", OracleDbType.Varchar2).Value = ext_po_num;
                            oracleCommand.Parameters.Add("unit_cost", OracleDbType.Decimal).Value = product.unit_cost;
                            oracleCommand.Parameters.Add("total_cost_local", OracleDbType.Decimal).Value = product.total_cost_local;

                            if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, "SP_IN_PO_DET_CREATE", false))
                            {
                                pruebaInternas.Add(new PruebaInterna()
                                {
                                    table = true,
                                    name = nameTable,
                                    id = codigo.ToString() + "-" + item,
                                    quantity = 1,
                                    message = ErrorMessage,
                                    method = "AddDataTable" + nameTable + "_det" + "-" + tipo
                                });
                            }
                            else
                            {
                                pruebaInternas.Add(new PruebaInterna()
                                {
                                    table = true,
                                    name = nameTable,
                                    id = codigo.ToString(),
                                    quantity = 0,
                                    message = ErrorMessage,
                                    method = "AddDataTable" + nameTable + "-" + tipo
                                });
                            }
                            //}
                        }
                    }
                    else
                    {
                        pruebaInternas.Add(new PruebaInterna()
                        {
                            table = true,
                            name = nameTable,
                            id = codigo.ToString(),
                            quantity = 0,
                            message = ErrorMessage,
                            method = "AddDataTable" + nameTable + "-" + tipo
                        });
                    }
                }
                else
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        table = true,
                        name = nameTable,
                        id = codigo.ToString(),
                        quantity = 0,
                        message = ErrorMessage,
                        method = "AddDataTable" + nameTable + "-" + tipo
                    });
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    table = true,
                    name = nameTable,
                    id = codigo.ToString(),
                    quantity = 0,
                    message = ex.Message,
                    method = "AddDataTable" + nameTable + "-" + tipo
                });
            }

            return pruebaInternas;
        }

        public List<PruebaInterna> AddDataTableIn_Comex(string nameTable, string tipo, DTOUnitario<In_Comex>.Request in_ComexRequest)
        {
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;
            List<PruebaInterna> pruebaInternas = new List<PruebaInterna>();
            int codigo = 0;
            try
            {
                foreach (var det_comex in in_ComexRequest.data.detalles)
                {
                    codigo++;
                    oracleCommand = new OracleCommand();
                    oracleCommand.Parameters.Add("OC", OracleDbType.Varchar2).Value = det_comex.oc;
                    oracleCommand.Parameters.Add("NUMERO1", OracleDbType.Varchar2).Value = det_comex.item;
                    oracleCommand.Parameters.Add("ESTILO1", OracleDbType.Varchar2).Value = det_comex.estilo;
                    oracleCommand.Parameters.Add("COD__PRODUCTO1", OracleDbType.Varchar2).Value = det_comex.cod_producto;
                    oracleCommand.Parameters.Add("CANTIDAD1", OracleDbType.Varchar2).Value = det_comex.cantidad;
                    
                    oracleCommand.Parameters.Add("CANT__FACTURA1", OracleDbType.Varchar2).Value = det_comex.cant_factura;
                    
                    oracleCommand.Parameters.Add("FCH_DELIVERY_DAY_1", OracleDbType.Varchar2).Value = det_comex.fch_delivery_day;
                    oracleCommand.Parameters.Add("FCH__ENTR__REAL", OracleDbType.Varchar2).Value = det_comex.fch_entrega_real == "" ? DBNull.Value : det_comex.fch_entrega_real;
                    
                    oracleCommand.Parameters.Add("FCH__SALIDA", OracleDbType.Varchar2).Value = det_comex.fch_salida == "" ? DBNull.Value : det_comex.fch_salida;
                    oracleCommand.Parameters.Add("FCH__LLEGADA", OracleDbType.Varchar2).Value = det_comex.fch_llegada == "" ? DBNull.Value :det_comex.fch_llegada;


                    if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, "SP_IN_COMEX_CREATE", false))
                    {
                        pruebaInternas.Add(new PruebaInterna()
                        {
                            table = true,
                            name = nameTable,
                            id = codigo.ToString() + "-" + det_comex,
                            quantity = 1,
                            message = ErrorMessage,
                            method = "AddDataTable" + nameTable + "_det" + "-" + tipo
                        });
                    }
                    else
                    {
                        pruebaInternas.Add(new PruebaInterna()
                        {
                            table = true,
                            name = nameTable,
                            id = codigo.ToString(),
                            quantity = 0,
                            message = ErrorMessage,
                            method = "AddDataTable" + nameTable + "-" + tipo
                        });
                    }


                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    table = true,
                    name = nameTable,
                    id = codigo.ToString(),
                    quantity = 0,
                    message = ex.Message,
                    method = "AddDataTable" + nameTable + "-" + tipo
                });
            }
            return pruebaInternas;
        }

        //AddDataTableIn_Comex
        public List<PruebaInterna> AddDataTableIn_Asn(string nameTable, string tipo, DTOUnitario<In_Asn>.Request in_AsnRequest)
        {
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;
            List<PruebaInterna> pruebaInternas = new List<PruebaInterna>();
            int codigo = 0;

            try
            {
                oracleCommand.Parameters.Add("asn", OracleDbType.Int32).Value = in_AsnRequest.data.asn;
                oracleCommand.Parameters.Add("folder_comex", OracleDbType.Varchar2).Value = in_AsnRequest.data.folder_comex.ToString();
                oracleCommand.Parameters.Add("container", OracleDbType.Varchar2).Value = in_AsnRequest.data.container == null ? "" : in_AsnRequest.data.container.ToString();
                oracleCommand.Parameters.Add("container_type", OracleDbType.Varchar2).Value = in_AsnRequest.data.container_type.ToString();

                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, "SP_IN_ASN_CAB_CREATE", true))
                {
                    if (ErrorMessage == "OK")
                    {
                        int item = 0;
                        int item_prod = 0;
                        pruebaInternas.Add(new PruebaInterna()
                        {
                            table = true,
                            name = nameTable,
                            id = codigo.ToString(),
                            quantity = 1,
                            message = ErrorMessage,
                            method = "AddDataTable" + nameTable + "_cab" + "-" + tipo
                        });
                        foreach (var lpn_item in in_AsnRequest.data.lpns)
                        {
                            item++;
                            oracleCommand = new OracleCommand();
                            oracleCommand.Parameters.Add("asn", OracleDbType.Int32).Value = in_AsnRequest.data.asn;
                            oracleCommand.Parameters.Add("lpn", OracleDbType.Varchar2).Value = lpn_item.lpn.ToString();
                            oracleCommand.Parameters.Add("cod_store", OracleDbType.Varchar2).Value = lpn_item.cod_store.ToString();
                            oracleCommand.Parameters.Add("package_type", OracleDbType.Varchar2).Value = lpn_item.package_type.ToString();
                            if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, "SP_IN_ASN_DET_LPN_CREATE", false))
                            {
                                pruebaInternas.Add(new PruebaInterna()
                                {
                                    table = true,
                                    name = nameTable,
                                    id = codigo.ToString() + "-" + item,
                                    quantity = 1,
                                    message = ErrorMessage,
                                    method = "AddDataTable" + nameTable + "_det" + "-" + tipo
                                });
                            }
                            else
                            {
                                pruebaInternas.Add(new PruebaInterna()
                                {
                                    table = true,
                                    name = nameTable,
                                    id = codigo.ToString(),
                                    quantity = 0,
                                    message = ErrorMessage,
                                    method = "AddDataTable" + nameTable + "-" + tipo
                                });
                            }
                            item_prod = 0;
                            foreach (var prod_lpn in lpn_item.products)
                            {
                                item_prod++;
                                oracleCommand = new OracleCommand();
                                oracleCommand.Parameters.Add("lpn", OracleDbType.Varchar2).Value = lpn_item.lpn.ToString();
                                oracleCommand.Parameters.Add("sku", OracleDbType.Varchar2).Value = prod_lpn.sku.ToString();
                                oracleCommand.Parameters.Add("purchase_order", OracleDbType.Int32).Value = prod_lpn.purchase_order;
                                oracleCommand.Parameters.Add("units", OracleDbType.Int32).Value = prod_lpn.units;
                                oracleCommand.Parameters.Add("packing_type_po", OracleDbType.Varchar2).Value = prod_lpn.packing_type_po.ToString();

                                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, "SP_IN_ASN_LPN_PROD_CREATE", false))
                                {
                                    pruebaInternas.Add(new PruebaInterna()
                                    {
                                        table = true,
                                        name = nameTable,
                                        id = codigo.ToString() + "-" + item_prod,
                                        quantity = 1,
                                        message = ErrorMessage,
                                        method = "AddDataTable" + nameTable + "_det_prod" + "-" + tipo
                                    });
                                }
                                else
                                {
                                    pruebaInternas.Add(new PruebaInterna()
                                    {
                                        table = true,
                                        name = nameTable,
                                        id = codigo.ToString(),
                                        quantity = 0,
                                        message = ErrorMessage,
                                        method = "AddDataTable" + nameTable + "-" + tipo
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        pruebaInternas.Add(new PruebaInterna()
                        {
                            table = true,
                            name = nameTable,
                            id = codigo.ToString(),
                            quantity = 0,
                            message = ErrorMessage,
                            method = "AddDataTable" + nameTable + "-" + tipo
                        });
                    }
                }
                else
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        table = true,
                        name = nameTable,
                        id = codigo.ToString(),
                        quantity = 0,
                        message = ErrorMessage,
                        method = "AddDataTable" + nameTable + "-" + tipo
                    });
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    table = true,
                    name = nameTable,
                    id = codigo.ToString(),
                    quantity = 0,
                    message = ex.Message,
                    method = "AddDataTable" + nameTable + "-" + tipo
                });
            }
            return pruebaInternas;
        }


        public List<PruebaInterna> AddDataTableIn_po(string nameTable, string tipo, DTOUnitario<In_Po>.Request in_PoRequest)
        {
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;
            List<PruebaInterna> pruebaInternas = new List<PruebaInterna>();
            int audit_number = 0;
            string ext_po_num = "";
            int codigo = 0;

            try
            {
                //SP_P_ADD_TPIMPOCC Y TABLA IN_PO_CAB
                oracleCommand.Parameters.Add("audit_number", OracleDbType.Int32).Direction = ParameterDirection.Output;
                oracleCommand.Parameters.Add("in_po_cab_id", OracleDbType.Int32).Direction = ParameterDirection.Output;
                oracleCommand.Parameters.Add("ext_po_num", OracleDbType.Varchar2, size: 50).Direction = ParameterDirection.Output;
                oracleCommand.Parameters.Add("id_document", OracleDbType.Varchar2).Value = in_PoRequest.data.id_document.ToString();
                oracleCommand.Parameters.Add("proforma_invoice", OracleDbType.Varchar2).Value = in_PoRequest.data.proforma_invoice;
                oracleCommand.Parameters.Add("po_type", OracleDbType.Varchar2).Value = in_PoRequest.data.po_type == "I" ? "OC Importado" : "OC Nacional";
                oracleCommand.Parameters.Add("distribution", OracleDbType.Int32).Value = in_PoRequest.data.distribution == "S" ? 2 : 6;
                oracleCommand.Parameters.Add("is_style_prepack", OracleDbType.Int32).Value = in_PoRequest.data.is_style_prepack == true ? 1 : 0;
                oracleCommand.Parameters.Add("vendor_id", OracleDbType.Varchar2).Value = in_PoRequest.data.vendor.id;
                oracleCommand.Parameters.Add("vendor_name", OracleDbType.Varchar2).Value = in_PoRequest.data.vendor.name;
                oracleCommand.Parameters.Add("sucursal", OracleDbType.Int32).Value = in_PoRequest.data.destination.id;
                oracleCommand.Parameters.Add("buyer_id", OracleDbType.Varchar2).Value = in_PoRequest.data.buyer.name;
                oracleCommand.Parameters.Add("buyer_name", OracleDbType.Varchar2).Value = in_PoRequest.data.buyer.name;
                oracleCommand.Parameters.Add("delivery_date", OracleDbType.Varchar2).Value = in_PoRequest.data.dates.delivery;
                oracleCommand.Parameters.Add("cancellation_date", OracleDbType.Varchar2).Value = in_PoRequest.data.dates.cancellation;
                oracleCommand.Parameters.Add("shipment_date", OracleDbType.Varchar2).Value = in_PoRequest.data.dates.shipment;
                oracleCommand.Parameters.Add("reception_date", OracleDbType.Varchar2).Value = in_PoRequest.data.dates.reception;
                oracleCommand.Parameters.Add("import_factor", OracleDbType.Decimal).Value = in_PoRequest.data.import_factor;
                oracleCommand.Parameters.Add("change_type", OracleDbType.Decimal).Value = in_PoRequest.data.change_type;
                oracleCommand.Parameters.Add("port_of_loading_id", OracleDbType.Varchar2).Value = in_PoRequest.data.port_of_loading.id;
                oracleCommand.Parameters.Add("port_of_loading_name", OracleDbType.Varchar2).Value = in_PoRequest.data.port_of_loading.name;
                oracleCommand.Parameters.Add("port_of_discharge_id", OracleDbType.Varchar2).Value = in_PoRequest.data.port_of_discharge.id;
                oracleCommand.Parameters.Add("port_of_discharge_name", OracleDbType.Varchar2).Value = in_PoRequest.data.port_of_discharge.name;
                oracleCommand.Parameters.Add("payment_terms", OracleDbType.Varchar2).Value = in_PoRequest.data.payment_terms.name;
                oracleCommand.Parameters.Add("incoterm", OracleDbType.Varchar2).Value = in_PoRequest.data.incoterm;
                oracleCommand.Parameters.Add("flg_complet", OracleDbType.Char).Value = "0";


                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, "SP_IN_PO_CAB_CREATE", true))
                {
                    if (ErrorMessage == "OK")
                    {
                        int item = 0;
                        audit_number = int.Parse(oracleCommand.Parameters["audit_number"].Value.ToString());
                        codigo = int.Parse(oracleCommand.Parameters["in_po_cab_id"].Value.ToString());
                        ext_po_num = oracleCommand.Parameters["ext_po_num"].Value.ToString();
                        pruebaInternas.Add(new PruebaInterna()
                        {
                            table = true,
                            name = nameTable,
                            id = codigo.ToString(),
                            quantity = 1,
                            message = ErrorMessage,
                            method = "AddDataTable" + nameTable + "_cab" + "-" + tipo
                        });
                        foreach (var product in in_PoRequest.data.products)
                        {
                            string sku = string.Empty;
                            oracleCommand = new OracleCommand();

                            //item++;
                            //oracleCommand = new OracleCommand();
                            for (int i = 0; i < (15 - product.sku.Length + 1); i++)
                            {
                                if (i == 0) sku = product.sku;
                                else sku += " ";
                            }

                            if (in_PoRequest.data.distribution == "S") //Simple
                            {
                                item++;

                                oracleCommand.Parameters.Add("audit_number_cab", OracleDbType.Int32).Value = audit_number;
                                oracleCommand.Parameters.Add("in_po_cab_id", OracleDbType.Int32).Value = codigo;
                                oracleCommand.Parameters.Add("line_number", OracleDbType.Int32).Value = item;
                                oracleCommand.Parameters.Add("sku", OracleDbType.Varchar2).Value = sku;
                                oracleCommand.Parameters.Add("org_lvl_number", OracleDbType.Int32).Value = in_PoRequest.data.destination.id;
                                oracleCommand.Parameters.Add("quantity", OracleDbType.Int32).Value = product.total_units;
                                oracleCommand.Parameters.Add("distribution", OracleDbType.Int32).Value = in_PoRequest.data.distribution == "S" ? 2 : 6;
                                oracleCommand.Parameters.Add("id_document", OracleDbType.Varchar2).Value = in_PoRequest.data.id_document.ToString();
                                oracleCommand.Parameters.Add("ext_po_num", OracleDbType.Varchar2).Value = ext_po_num;
                                oracleCommand.Parameters.Add("unit_cost", OracleDbType.Decimal).Value = product.unit_cost;
                                oracleCommand.Parameters.Add("total_cost_local", OracleDbType.Decimal).Value = product.total_cost_local;

                                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, "SP_IN_PO_DET_CREATE", false))
                                {
                                    pruebaInternas.Add(new PruebaInterna()
                                    {
                                        table = true,
                                        name = nameTable,
                                        id = codigo.ToString() + "-" + item,
                                        quantity = 1,
                                        message = ErrorMessage,
                                        method = "AddDataTable" + nameTable + "_det" + "-" + tipo
                                    });
                                }
                                else
                                {
                                    pruebaInternas.Add(new PruebaInterna()
                                    {
                                        table = true,
                                        name = nameTable,
                                        id = codigo.ToString(),
                                        quantity = 0,
                                        message = ErrorMessage,
                                        method = "AddDataTable" + nameTable + "-" + tipo
                                    });
                                }

                            }
                            else //Predistribuido
                            {
                                foreach (var destination in product.destination)
                                {
                                    item++;

                                    oracleCommand = new OracleCommand();

                                    oracleCommand.Parameters.Add("audit_number_cab", OracleDbType.Int32).Value = audit_number;
                                    oracleCommand.Parameters.Add("in_po_cab_id", OracleDbType.Int32).Value = codigo;
                                    oracleCommand.Parameters.Add("line_number", OracleDbType.Int32).Value = item;
                                    oracleCommand.Parameters.Add("sku", OracleDbType.Varchar2).Value = sku;
                                    oracleCommand.Parameters.Add("org_lvl_number", OracleDbType.Int32).Value = destination.id; //in_PoRequest.data.destination.id;
                                    oracleCommand.Parameters.Add("quantity", OracleDbType.Int32).Value = destination.units;//product.total_units;
                                    oracleCommand.Parameters.Add("distribution", OracleDbType.Int32).Value = in_PoRequest.data.distribution == "S" ? 2 : 6;
                                    oracleCommand.Parameters.Add("id_document", OracleDbType.Varchar2).Value = in_PoRequest.data.id_document.ToString();
                                    oracleCommand.Parameters.Add("ext_po_num", OracleDbType.Varchar2).Value = ext_po_num;
                                    oracleCommand.Parameters.Add("unit_cost", OracleDbType.Decimal).Value = product.unit_cost;
                                    oracleCommand.Parameters.Add("total_cost_local", OracleDbType.Decimal).Value = product.total_cost_local;

                                    if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, "SP_IN_PO_DET_CREATE", false))
                                    {
                                        pruebaInternas.Add(new PruebaInterna()
                                        {
                                            table = true,
                                            name = nameTable,
                                            id = codigo.ToString() + "-" + item,
                                            quantity = 1,
                                            message = ErrorMessage,
                                            method = "AddDataTable" + nameTable + "_det" + "-" + tipo
                                        });
                                    }
                                    else
                                    {
                                        pruebaInternas.Add(new PruebaInterna()
                                        {
                                            table = true,
                                            name = nameTable,
                                            id = codigo.ToString(),
                                            quantity = 0,
                                            message = ErrorMessage,
                                            method = "AddDataTable" + nameTable + "-" + tipo
                                        });
                                    }
                                }
                            }
                            //}
                        }
                        // actualizamos el flag
                        oracleCommand = new OracleCommand();
                        oracleCommand.Parameters.Add("in_id_document", OracleDbType.Varchar2).Value = in_PoRequest.data.id_document.ToString();
                        oracleCommand.Parameters.Add("in_flg_complet", OracleDbType.Char).Value = "1";

                        if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, "SP_IN_PO_CAB_FLAG_COMPLETE", false))
                        {
                            pruebaInternas.Add(new PruebaInterna()
                            {
                                table = true,
                                name = nameTable,
                                id = codigo.ToString(),
                                quantity = 1,
                                message = ErrorMessage,
                                method = "AddDataTable" + nameTable + "-" + "completar OC"
                            });
                        }
                        else
                        {
                            pruebaInternas.Add(new PruebaInterna()
                            {
                                table = true,
                                name = nameTable,
                                id = codigo.ToString(),
                                quantity = 0,
                                message = ErrorMessage,
                                method = "AddDataTable" + nameTable + "-" + "completar OC"
                            });
                        }
                    }
                    else
                    {
                        pruebaInternas.Add(new PruebaInterna()
                        {
                            table = true,
                            name = nameTable,
                            id = codigo.ToString(),
                            quantity = 0,
                            message = ErrorMessage,
                            method = "AddDataTable" + nameTable + "-" + tipo
                        });
                    }
                }
                else
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        table = true,
                        name = nameTable,
                        id = codigo.ToString(),
                        quantity = 0,
                        message = ErrorMessage,
                        method = "AddDataTable" + nameTable + "-" + tipo
                    });
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    table = true,
                    name = nameTable,
                    id = codigo.ToString(),
                    quantity = 0,
                    message = ex.Message,
                    method = "AddDataTable" + nameTable + "-" + tipo
                });
            }

            return pruebaInternas;
        }

        public List<PruebaInterna> AddDataTableIn_Codes(string nameTable, DTOUnitario<In_Codes>.Request in_CodesRequest, int tipo)
        {
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;
            List<PruebaInterna> pruebaInternas = new List<PruebaInterna>();
            string codigo = string.Empty;
            List<In_Codes.GsAttr> AtributosTemporales = new List<In_Codes.GsAttr>();
            string arrayTemporal;

            try
            {
                codigo = in_CodesRequest.data.master_code.ToString();
                //TABLA IN_CODES_CAB
                oracleCommand.Parameters.Add("master_code", OracleDbType.Varchar2).Value = codigo;
                oracleCommand.Parameters.Add("is_unique_size", OracleDbType.Varchar2).Value = in_CodesRequest.data.is_unique_size == true ? "1" : "0";
                oracleCommand.Parameters.Add("parent_sku", OracleDbType.Varchar2).Value = in_CodesRequest.data.parent_sku == null ? "" : in_CodesRequest.data.parent_sku;
                oracleCommand.Parameters.Add("cod_linea", OracleDbType.Varchar2).Value = in_CodesRequest.data.line.id.ToString();
                oracleCommand.Parameters.Add("description", OracleDbType.Varchar2).Value = in_CodesRequest.data.description.ToString();
                oracleCommand.Parameters.Add("brand_id", OracleDbType.Varchar2).Value = in_CodesRequest.data.brand.id.ToString();
                oracleCommand.Parameters.Add("brand_desc", OracleDbType.Varchar2).Value = in_CodesRequest.data.brand.name.ToString();
                //oracleCommand.Parameters.Add("dim_type_code", OracleDbType.Varchar2).Value = tipo != 1 ? "" : in_CodesRequest.data.dim_type.code;
                //oracleCommand.Parameters.Add("dim_type_desc", OracleDbType.Varchar2).Value = tipo != 1 ? "" : in_CodesRequest.data.dim_type.name;
                oracleCommand.Parameters.Add("dim_type_code", OracleDbType.Varchar2).Value = in_CodesRequest.data.dim_type == null ? "" : in_CodesRequest.data.dim_type.code;
                oracleCommand.Parameters.Add("dim_type_desc", OracleDbType.Varchar2).Value = in_CodesRequest.data.dim_type == null ? "" : in_CodesRequest.data.dim_type.name;
                oracleCommand.Parameters.Add("size_type_code", OracleDbType.Varchar2).Value = in_CodesRequest.data.size_type_id == null ? "" : in_CodesRequest.data.size_type_id.id; ;//ToDo: Falata en la especificacion
                oracleCommand.Parameters.Add("size_type_desc", OracleDbType.Varchar2).Value = in_CodesRequest.data.size_type_id == null ? "" : in_CodesRequest.data.size_type_id.name; ;//ToDo: Falata en la especificacion
                oracleCommand.Parameters.Add("packing_type", OracleDbType.Varchar2).Value = in_CodesRequest.data.packing_type == null ? "" : in_CodesRequest.data.packing_type; ;//ToDo: Falata en la especificacion
                oracleCommand.Parameters.Add("flg_complet", OracleDbType.Char).Value = "0";//ToDo: Falata en la especificacion


                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, "SP_IN_CODES_MASTER_CREATE", false))
                {
                    if (ErrorMessage == "OK")
                    {
                        pruebaInternas.Add(new PruebaInterna()
                        {
                            table = true,
                            name = nameTable,
                            id = codigo.ToString(),
                            quantity = 1,
                            message = ErrorMessage,
                            method = "AddDataTable" + nameTable + "-" + "MASTER"
                        });

                        //if (tipo == 1)
                        //{
                        for (int i = 0; i < in_CodesRequest.data.dim_values.Count; i++)
                        {
                            if (i == 0)
                            {
                                for (int j = 0; j < in_CodesRequest.data.sizes.Count; j++)
                                {
                                    oracleCommand = new OracleCommand();
                                    oracleCommand.Parameters.Add("master_code", OracleDbType.Varchar2).Value = codigo;
                                    oracleCommand.Parameters.Add("size_desc", OracleDbType.Varchar2).Value = in_CodesRequest.data.sizes[j];
                                    oracleCommand.Parameters.Add("size_ratio", OracleDbType.Int16).Value = in_CodesRequest.data.dim_values[i].ratio[j];

                                    if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, "SP_IN_CODES_SIZES_CREATE", false))
                                    {
                                        pruebaInternas.Add(new PruebaInterna()
                                        {
                                            table = true,
                                            name = nameTable,
                                            id = codigo.ToString(),
                                            quantity = 1,
                                            message = ErrorMessage,
                                            method = "AddDataTable" + nameTable + "-" + "SIZES"
                                        });
                                    }
                                    else
                                    {
                                        pruebaInternas.Add(new PruebaInterna()
                                        {
                                            table = true,
                                            name = nameTable,
                                            id = codigo.ToString(),
                                            quantity = 0,
                                            message = ErrorMessage,
                                            method = "AddDataTable" + nameTable + "-" + "SIZES"
                                        });
                                    }
                                }
                            }

                            oracleCommand = new OracleCommand();
                            oracleCommand.Parameters.Add("master_code", OracleDbType.Varchar2).Value = codigo;
                            oracleCommand.Parameters.Add("bbook_id_prd_color", OracleDbType.Varchar2).Value = in_CodesRequest.data.dim_values[i].id;
                            oracleCommand.Parameters.Add("color_desc", OracleDbType.Varchar2).Value = in_CodesRequest.data.dim_values[i].name;

                            if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, "SP_IN_CODES_COLORS_CREATE", false))
                            {
                                pruebaInternas.Add(new PruebaInterna()
                                {
                                    table = true,
                                    name = nameTable,
                                    id = codigo.ToString(),
                                    quantity = 1,
                                    message = ErrorMessage,
                                    method = "AddDataTable" + nameTable + "-" + "COLORS"
                                });
                            }
                            else
                            {
                                pruebaInternas.Add(new PruebaInterna()
                                {
                                    table = true,
                                    name = nameTable,
                                    id = codigo.ToString(),
                                    quantity = 1,
                                    message = ErrorMessage,
                                    method = "AddDataTable" + nameTable + "-" + "COLORS"
                                });
                            }
                            if (i == 0)
                            {
                                foreach (var gs_attr in in_CodesRequest.data.dim_values[i].gs_attrs)
                                {
                                    oracleCommand = new OracleCommand();
                                    oracleCommand.Parameters.Add("master_code", OracleDbType.Varchar2).Value = codigo;
                                    oracleCommand.Parameters.Add("attr_code", OracleDbType.Varchar2).Value = gs_attr.code;
                                    oracleCommand.Parameters.Add("attr_name", OracleDbType.Varchar2).Value = gs_attr.name;
                                    if (gs_attr.code == "ean_proveedor" && gs_attr.value.ToString().Length > 13) oracleCommand.Parameters.Add("attr_value", OracleDbType.Varchar2).Value = (gs_attr.value).ToString().Split(",")[0].Trim();
                                    else if (gs_attr.code == "Case_Pack") oracleCommand.Parameters.Add("attr_value", OracleDbType.Varchar2).Value = (gs_attr.value).ToString().Split(",")[0].Trim();
                                    else oracleCommand.Parameters.Add("attr_value", OracleDbType.Varchar2).Value = gs_attr.value;

                                    if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, "SP_IN_CODES_ATTRS_CREATE", false))
                                    {
                                        pruebaInternas.Add(new PruebaInterna()
                                        {
                                            table = true,
                                            name = nameTable,
                                            id = codigo.ToString(),
                                            quantity = 1,
                                            message = ErrorMessage,
                                            method = "AddDataTable" + nameTable + "-" + "ATTRS"
                                        });
                                    }
                                    else
                                    {
                                        pruebaInternas.Add(new PruebaInterna()
                                        {
                                            table = true,
                                            name = nameTable,
                                            id = codigo.ToString(),
                                            quantity = 0,
                                            message = ErrorMessage,
                                            method = "AddDataTable" + nameTable + "-" + "ATTRS"
                                        });
                                    }
                                }
                                AtributosTemporales = in_CodesRequest.data.dim_values[i].gs_attrs.FindAll(x => x.code == "A83"
                                                                                                             || x.code == "A84");
                                if (AtributosTemporales.Count < 2)
                                {
                                    AtributosTemporales = new List<In_Codes.GsAttr>();

                                    if (!in_CodesRequest.data.dim_values[i].gs_attrs.Exists(x => x.code == "A83"))
                                    {
                                        arrayTemporal = string.Empty;
                                        for (int j = 0; j < in_CodesRequest.data.sizes.Count; j++)
                                        {
                                            arrayTemporal += in_CodesRequest.data.sizes[j].Trim();
                                            if (j < in_CodesRequest.data.sizes.Count - 1) arrayTemporal += " ";
                                        }
                                        arrayTemporal = arrayTemporal.Replace(" ", "-");
                                        AtributosTemporales.Add(new In_Codes.GsAttr() { code = "A83", name = "TALLA", value = arrayTemporal });
                                    }
                                    if (!in_CodesRequest.data.dim_values[i].gs_attrs.Exists(x => x.code == "A84"))
                                    {
                                        arrayTemporal = string.Empty;
                                        for (int j = 0; j < in_CodesRequest.data.dim_values[i].ratio.Count; j++)
                                        {
                                            arrayTemporal += in_CodesRequest.data.dim_values[i].ratio[j].ToString().Trim();
                                            if (j < in_CodesRequest.data.dim_values[i].ratio.Count - 1) arrayTemporal += " ";
                                        }
                                        arrayTemporal = arrayTemporal.Replace(" ", "-");
                                        AtributosTemporales.Add(new In_Codes.GsAttr() { code = "A84", name = "CURVA", value = arrayTemporal });
                                    }

                                    foreach (var gs_attr in AtributosTemporales)
                                    {
                                        oracleCommand = new OracleCommand();
                                        oracleCommand.Parameters.Add("master_code", OracleDbType.Varchar2).Value = codigo;
                                        oracleCommand.Parameters.Add("attr_code", OracleDbType.Varchar2).Value = gs_attr.code;
                                        oracleCommand.Parameters.Add("attr_name", OracleDbType.Varchar2).Value = gs_attr.name;
                                        oracleCommand.Parameters.Add("attr_value", OracleDbType.Varchar2).Value = gs_attr.value;

                                        if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, "SP_IN_CODES_ATTRS_CREATE", false))
                                        {
                                            pruebaInternas.Add(new PruebaInterna()
                                            {
                                                table = true,
                                                name = nameTable,
                                                id = codigo.ToString(),
                                                quantity = 1,
                                                message = ErrorMessage,
                                                method = "AddDataTable" + nameTable + "-" + "ATTRS OPC"
                                            });
                                        }
                                        else
                                        {
                                            pruebaInternas.Add(new PruebaInterna()
                                            {
                                                table = true,
                                                name = nameTable,
                                                id = codigo.ToString(),
                                                quantity = 0,
                                                message = ErrorMessage,
                                                method = "AddDataTable" + nameTable + "-" + "ATTRS OPC"
                                            });
                                        }
                                    }
                                }
                            }
                        }
                        //llamar al proc de compeltar producto
                        oracleCommand = new OracleCommand();
                        oracleCommand.Parameters.Add("in_master_code", OracleDbType.Varchar2).Value = codigo;
                        oracleCommand.Parameters.Add("in_flg_complet", OracleDbType.Char).Value = "1";

                        if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, "SP_IN_CODES_FLAG_COMPLETE", false))
                        {
                            pruebaInternas.Add(new PruebaInterna()
                            {
                                table = true,
                                name = nameTable,
                                id = codigo.ToString(),
                                quantity = 1,
                                message = ErrorMessage,
                                method = "AddDataTable" + nameTable + "-" + "completar producto"
                            });
                        }
                        else
                        {
                            pruebaInternas.Add(new PruebaInterna()
                            {
                                table = true,
                                name = nameTable,
                                id = codigo.ToString(),
                                quantity = 0,
                                message = ErrorMessage,
                                method = "AddDataTable" + nameTable + "-" + "completar producto"
                            });
                        }
                        //}
                        //else
                        //{
                        //    foreach (var gs_attr in in_CodesRequest.data.gs_attrs)
                        //    {
                        //        oracleCommand = new OracleCommand();
                        //        oracleCommand.Parameters.Add("master_code", OracleDbType.Varchar2).Value = codigo;
                        //        oracleCommand.Parameters.Add("attr_code", OracleDbType.Varchar2).Value = gs_attr.code;
                        //        oracleCommand.Parameters.Add("attr_name", OracleDbType.Varchar2).Value = gs_attr.name;
                        //        if (gs_attr.code == "ean_proveedor" && gs_attr.value.ToString().Length > 13) oracleCommand.Parameters.Add("attr_value", OracleDbType.Varchar2).Value = (gs_attr.value).ToString().Split(",")[0].Trim();
                        //        if (gs_attr.code == "Case_Pack") oracleCommand.Parameters.Add("attr_value", OracleDbType.Varchar2).Value = (gs_attr.value).ToString().Split(",")[0].Trim();
                        //        else oracleCommand.Parameters.Add("attr_value", OracleDbType.Varchar2).Value = gs_attr.value;

                        //        if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, "SP_IN_CODES_ATTRS_CREATE", false))
                        //        {
                        //            pruebaInternas.Add(new PruebaInterna()
                        //            {
                        //                table = true,
                        //                name = nameTable,
                        //                id = codigo.ToString(),
                        //                quantity = 1,
                        //                message = ErrorMessage,
                        //                method = "AddDataTable" + nameTable + "-" + "ATTRS"
                        //            });
                        //        }
                        //        else
                        //        {
                        //            pruebaInternas.Add(new PruebaInterna()
                        //            {
                        //                table = true,
                        //                name = nameTable,
                        //                id = codigo.ToString(),
                        //                quantity = 0,
                        //                message = ErrorMessage,
                        //                method = "AddDataTable" + nameTable + "-" + "ATTRS"
                        //            });
                        //        }
                        //    }
                        //}
                    }
                    else
                    {
                        pruebaInternas.Add(new PruebaInterna()
                        {
                            table = true,
                            name = nameTable,
                            id = codigo.ToString(),
                            quantity = 0,
                            message = ErrorMessage,
                            method = "AddDataTable" + nameTable + "-" + "MASTER"
                        });

                    }
                }
                else
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        table = true,
                        name = nameTable,
                        id = codigo.ToString(),
                        quantity = 0,
                        message = ErrorMessage,
                        method = "AddDataTable" + nameTable + "-" + "MASTER"
                    });
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    table = true,
                    name = nameTable,
                    id = codigo.ToString(),
                    quantity = 0,
                    message = ex.Message,
                    method = "AddDataTable" + nameTable + "-" + tipo
                });
            }
            return pruebaInternas;
        }
        public List<PruebaInterna> AddDataTableProduct_Modification(string nameTable, string tipo, DTO<Product_modification>.Request product_ModificationRequest)
        {
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;
            List<PruebaInterna> pruebaInternas = new List<PruebaInterna>();

            foreach (var item in product_ModificationRequest.data)
            {
                try
                {
                    oracleCommand = new OracleCommand();
                    oracleCommand.Parameters.Add("parent_sku", OracleDbType.Varchar2).Value = item.parent_sku.ToString();
                    oracleCommand.Parameters.Add("id_prdmod", OracleDbType.Int32).Direction = ParameterDirection.Output;

                    if (_iDBOracleRepository.EjecutaSPBbook2(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, "SP_PRODUCT_MOD_MASTER_CREATE", false))
                    {
                        if (ErrorMessage == "OK")
                        {
                            int codigo = int.Parse(oracleCommand.Parameters["id_prdmod"].Value.ToString());
                            pruebaInternas.Add(new PruebaInterna()
                            {
                                table = true,
                                name = nameTable,
                                id = item.parent_sku.ToString(),
                                quantity = 1,
                                message = ErrorMessage,
                                method = "AddDataTable" + nameTable + "-" + "MASTER"
                            });
                            foreach (var attribute in item.attributes)
                            {
                                oracleCommand = new OracleCommand();
                                oracleCommand.Parameters.Add("parent_sku", OracleDbType.Varchar2).Value = item.parent_sku;
                                oracleCommand.Parameters.Add("attr_code", OracleDbType.Varchar2).Value = attribute.code;
                                oracleCommand.Parameters.Add("attr_name", OracleDbType.Varchar2).Value = attribute.name;
                                oracleCommand.Parameters.Add("attr_value", OracleDbType.Varchar2).Value = attribute.value;
                                oracleCommand.Parameters.Add("id_prdmod", OracleDbType.Int32).Value = codigo;

                                if (_iDBOracleRepository.EjecutaSPBbook2(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, "SP_PRODUCT_MOD_ATTRS_CREATE", false))
                                {
                                    pruebaInternas.Add(new PruebaInterna()
                                    {
                                        table = true,
                                        name = nameTable,
                                        id = item.parent_sku.ToString() + "-" + attribute.code,
                                        quantity = 1,
                                        message = ErrorMessage,
                                        method = "AddDataTable" + nameTable + "_det" + "-" + tipo
                                    });
                                }
                                else
                                {
                                    pruebaInternas.Add(new PruebaInterna()
                                    {
                                        table = true,
                                        name = nameTable,
                                        id = item.parent_sku.ToString(),
                                        quantity = 0,
                                        message = ErrorMessage,
                                        method = "AddDataTable" + nameTable + "-" + tipo
                                    });
                                }
                            }
                        }
                        else
                        {
                            pruebaInternas.Add(new PruebaInterna()
                            {
                                table = true,
                                name = nameTable,
                                id = item.parent_sku.ToString(),
                                quantity = 0,
                                message = ErrorMessage,
                                method = "AddDataTable" + nameTable + "-" + tipo
                            });
                        }
                    }
                    else
                    {
                        pruebaInternas.Add(new PruebaInterna()
                        {
                            table = true,
                            name = nameTable,
                            id = item.parent_sku.ToString(),
                            quantity = 0,
                            message = ErrorMessage,
                            method = "AddDataTable" + nameTable + "-" + tipo
                        });
                    }
                }
                catch (Exception ex)
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        table = true,
                        name = nameTable,
                        id = item.parent_sku.ToString(),
                        quantity = 0,
                        message = ex.Message,
                        method = "AddDataTable" + nameTable + "-" + tipo
                    });
                }
            }
            return pruebaInternas;
        }

        public bool IsPrepack(string sku, ref string ErrorMessage)
        {
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;

            try
            {
                oracleCommand.Parameters.Add("sku", OracleDbType.Varchar2).Value = sku.Trim();
                oracleCommand.Parameters.Add("In_pos_C", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, "sp_search_isprepack", true))
                {
                    if (oracleDataReader.Read())
                    {
                        ErrorMessage = "OK";
                        if (int.Parse(oracleDataReader["Cod_Est"].ToString().Trim()) == 99) return true;
                        else return false;
                    }
                    else
                    {
                        ErrorMessage = "No se encontró registro con ese sku";
                        return false;
                    }
                }
                else
                {
                    ErrorMessage = "ERROR EN CONEXION A BASE DE DATOS";
                    return false;
                }
            }
            catch (Exception ex)
            {

                ErrorMessage = ex.Message;
                return false;
            }
        }
        public bool DataOKIn_Comex(ref string ErrorMessage, DTOUnitario<In_Comex>.Request in_ComexRequest)
        {
            bool OK = true;
            if (in_ComexRequest.data != null)
            {
                if (in_ComexRequest.data.detalles != null && in_ComexRequest.data.detalles.Count > 0)
                {
                    Int32 value = 0;
                    Regex validateDateRegex = new Regex("^[0-9]{1,2}\\/[0-9]{1,2}\\/[0-9]{4}$");
                    foreach (var item in in_ComexRequest.data.detalles)
                    {
                        if (item.oc == "")
                        {
                            OK = false;
                            ErrorMessage = "Data (Detalles) OC Nulo";
                            break;
                        }
                        if (item.item == "")
                        {
                            OK = false;
                            ErrorMessage = "Data (Detalles) Item Nulo";
                            break;
                        }
                        /*
                        Int32.TryParse(item.estilo, out value);
                        if (value == 0)
                        {
                            OK = false;
                            ErrorMessage = "Data (Detalles) Estilo Nulo";
                            break;
                        }*/
                        Int32.TryParse(item.cod_producto, out value);
                        if (value == 0)
                        {
                            OK = false;
                            ErrorMessage = "Data (Detalles) cod_producto Nulo";
                            break;
                        }
                        /*if (item.cantidad == 0)
                        {
                            OK = false;
                            ErrorMessage = "Data (Detalles) Cantidad Nulo";
                            break;
                        }*/
                        /*
                        if (!validateDateRegex.IsMatch(item.fch_entrega_real))
                        {
                            OK = false;
                            ErrorMessage = "Data (Detalles) Fecha entrega real Nulo";
                            break;
                        } */
                        if (!validateDateRegex.IsMatch(item.fch_delivery_day))
                        {
                            OK = false;
                            ErrorMessage = "Data (Detalles) Fecha Delivery day Nulo";
                            break;
                        }
                    }
                }
                else
                {
                    ErrorMessage = "Data (Detalles) Nulo";
                    OK = false;
                }
            }
            else
            {
                ErrorMessage = "Valores nulos en data";
                OK = false;
            }
            return OK;
        }
        public bool DataOKIn_Asn(ref string ErrorMessage, DTOUnitario<In_Asn>.Request in_AsnRequest)
        {
            bool OK = false;
            if (in_AsnRequest.data != null)
            {
                #region unitario
                if (in_AsnRequest.data.asn == 0)
                {
                    ErrorMessage = "Asn nro: Nulo";
                    return false;
                }
                if (in_AsnRequest.data.folder_comex == string.Empty || in_AsnRequest.data.folder_comex == null)
                {
                    ErrorMessage = "folder comex nro: Nulo";
                    return false;
                }
                if (in_AsnRequest.data.container_type == string.Empty || in_AsnRequest.data.container_type == null)
                {
                    ErrorMessage = "Tipo de contenedor Nulo";
                    return false;
                }

                if (in_AsnRequest.data.lpns != null && in_AsnRequest.data.lpns.Count > 0)
                {
                    foreach (var item in in_AsnRequest.data.lpns)
                    {
                        if (item.lpn == null || item.lpn == string.Empty)
                        {
                            ErrorMessage = "data-lpn Nulo";
                            return false;
                        }
                        if (item.cod_store == 0)
                        {
                            ErrorMessage = "data-cod_store Nulo";
                            return false;
                        }
                        if (item.package_type == string.Empty || item.package_type == null)
                        {
                            ErrorMessage = "data-tipo de package en LPN nulo";
                            return false;
                        }

                        if (item.products != null && item.products.Count > 0)
                        {
                            foreach (var prod_lpn in item.products)
                            {
                                if (prod_lpn.purchase_order == 0)
                                {
                                    ErrorMessage = "data-products-purchase_order Nulo";
                                    return false;
                                }
                                if (prod_lpn.sku == null || prod_lpn.sku == string.Empty)
                                {
                                    ErrorMessage = "data-products-sku Nulo";
                                    return false;
                                }
                                if (prod_lpn.units == 0)
                                {
                                    ErrorMessage = "data-products-units Nulo";
                                    return false;
                                }
                                if (prod_lpn.packing_type_po == string.Empty || prod_lpn.packing_type_po == null)
                                {
                                    ErrorMessage = "data-products-tipo de package en producto nulo";
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            ErrorMessage = "data-products Nulo";
                            return false;
                        }
                    }
                }
                else
                {
                    ErrorMessage = "Data (LPN) Nulo";
                    return false;
                }

                #endregion unitario

            }
            else
            {
                ErrorMessage = "Valores nulos en data";
                return false;
            }
            return true;
        }

        public bool DataOKIn_po(ref string ErrorMessage, DTOUnitario<In_Po>.Request in_PoRequest)
        {
            bool OK = false;
            if (in_PoRequest.data != null)
            {
                #region unitario
                if (in_PoRequest.data.id_document == 0)
                {
                    ErrorMessage = "Id_Document Nulo";
                    return false;
                }
                else if (in_PoRequest.data.proforma_invoice == string.Empty || in_PoRequest.data.proforma_invoice == null)
                {
                    ErrorMessage = "Proforma_invoice Nulo";
                    return false;
                }
                else if (in_PoRequest.data.po_type == string.Empty || in_PoRequest.data.po_type == null)
                {
                    ErrorMessage = "Po_type Nulo";
                    return false;
                }
                else if (in_PoRequest.data.distribution == string.Empty || in_PoRequest.data.distribution == null)
                {
                    ErrorMessage = "Distribucion Nulo";
                    return false;
                }
                else if (in_PoRequest.data.import_factor == 0)
                {
                    ErrorMessage = "Factor Importacion Nulo";
                    return false;
                }
                else if (in_PoRequest.data.change_type == 0)
                {
                    ErrorMessage = "Tipo de cambio Nulo";
                    return false;
                }
                else if (in_PoRequest.data.payment_terms.name == string.Empty || in_PoRequest.data.payment_terms.name == null)
                {
                    ErrorMessage = "Metodo Pago Nulo";
                    return false;
                }
                else if ((in_PoRequest.data.po_type == "I") && (in_PoRequest.data.incoterm == string.Empty || in_PoRequest.data.incoterm == null))
                {
                    ErrorMessage = "Incoterm Nulo";
                    return false;
                }
                else if (in_PoRequest.data.currency == string.Empty || in_PoRequest.data.currency == null)
                {
                    ErrorMessage = "Moneda Nulo";
                    return false;
                }
                #endregion unitario
                #region compuestos
                if (in_PoRequest.data.vendor != null)
                {
                    if (in_PoRequest.data.vendor.id == string.Empty || in_PoRequest.data.vendor.id == null)
                    {
                        ErrorMessage = "Id Vendedor Nulo";
                        return false;
                    }
                }
                else
                {
                    ErrorMessage = "Vendedor Nulo";
                    return false;
                }
                if (in_PoRequest.data.buyer != null)
                {
                    if (in_PoRequest.data.buyer.id == string.Empty || in_PoRequest.data.buyer.id == null)
                    {
                        ErrorMessage = "Id Comprador Nulo";
                        return false;
                    }
                }
                else
                {
                    ErrorMessage = "Comprador Nulo";
                    return false;
                }
                if (in_PoRequest.data.dates != null)
                {
                    switch (in_PoRequest.data.po_type)
                    {
                        case "I":
                            if (in_PoRequest.data.dates.delivery == string.Empty || in_PoRequest.data.dates.delivery == null)
                            {
                                ErrorMessage = "Fecha Entrega Nulo";
                                return false;
                            }
                            else if (in_PoRequest.data.dates.reception == string.Empty || in_PoRequest.data.dates.reception == null)
                            {
                                ErrorMessage = "Fecha recepcion Nulo";
                                return false;
                            }
                            else if (in_PoRequest.data.dates.shipment == string.Empty || in_PoRequest.data.dates.shipment == null)
                            {
                                ErrorMessage = "Fecha envio(shipment) Nulo";
                                return false;
                            }
                            break;
                        case "N":
                            if (in_PoRequest.data.dates.reception == string.Empty || in_PoRequest.data.dates.reception == null)
                            {
                                ErrorMessage = "Fecha recepcion Nulo";
                                return false;
                            }
                            break;
                    }

                    if (in_PoRequest.data.dates.cancellation == string.Empty || in_PoRequest.data.dates.cancellation == null)
                    {
                        ErrorMessage = "Fecha Cancelacion Nulo";
                        return false;
                    }
                }
                else
                {
                    ErrorMessage = "Fechas Nulas";
                    return false;
                }
                if (in_PoRequest.data.po_type == "I")
                {
                    if (in_PoRequest.data.port_of_loading != null)
                    {
                        if (in_PoRequest.data.port_of_loading.id == string.Empty || in_PoRequest.data.port_of_loading.id == null)
                        {
                            ErrorMessage = "Id Puerto Carga Nulo";
                            return false;
                        }
                    }
                    else
                    {
                        ErrorMessage = "Puerto Carga Nulo";
                        return false;
                    }
                    if (in_PoRequest.data.port_of_discharge != null)
                    {
                        if (in_PoRequest.data.port_of_discharge.id == string.Empty || in_PoRequest.data.port_of_discharge.id == null)
                        {
                            ErrorMessage = "Id Puerto Descarga Nulo";
                            return false;
                        }
                    }
                    else
                    {
                        ErrorMessage = "Puerto Descarga Nulo";
                        return false;
                    }
                }
                if (in_PoRequest.data.destination != null)
                {
                    if (in_PoRequest.data.destination.id == string.Empty || in_PoRequest.data.destination.id == null)
                    {
                        ErrorMessage = "Id Destino Principal Nulo";
                        return false;
                    }
                }
                else
                {
                    ErrorMessage = "Destino Principal Nulo";
                    return false;
                }
                if (in_PoRequest.data.products != null && in_PoRequest.data.products.Count > 0)
                {
                    foreach (var item in in_PoRequest.data.products)
                    {
                        if (item.sku == null || item.sku == string.Empty)
                        {
                            ErrorMessage = "Producto-Sku Nulo";
                            return false;
                        }
                        else if (item.unit_cost == 0)
                        {
                            ErrorMessage = "Producto-Costo Unitario Nulo";
                            return false;
                        }
                        else if (item.total_cost_local == 0)
                        {
                            ErrorMessage = "total_cost_local Nulo";
                            return false;
                        }
                        if (item.destination != null && item.destination.Count > 0)
                        {
                            foreach (var destination in item.destination)
                            {
                                if (destination.id == null || destination.id == string.Empty)
                                {
                                    ErrorMessage = "Id Destino-Producto Nulo";
                                    return false;
                                }
                                else if (destination.units == 0)
                                {
                                    ErrorMessage = "Unidades Destino-Producto Nulo";
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            ErrorMessage = "Destinos-Producto Nulo";
                            return false;
                        }
                        OK = IsPrepack(item.sku, ref ErrorMessage);
                        if (ErrorMessage != "OK") break;
                        if (OK != in_PoRequest.data.is_style_prepack && ErrorMessage == "OK") break;
                    }
                    if (ErrorMessage != "OK" && ErrorMessage != null)
                    {
                        return false;
                    }
                    if (OK != in_PoRequest.data.is_style_prepack)
                    {
                        ErrorMessage = "Prepack distinto entre cabecera y productos";
                        return false;
                    }
                }
                else
                {
                    ErrorMessage = "Productos Nulo";
                    return false;
                }

                if (in_PoRequest.data.distribution == "S" && in_PoRequest.data.destination.id == "902")
                {
                    ErrorMessage = "No esta permitido generar OC con metodo de distribucion Simple en 902-CD Oriente";
                    return false;
                }

                if (in_PoRequest.data.distribution != "S" && in_PoRequest.data.is_style_prepack == true)//P:predistribuida, S:Simple
                {
                    ErrorMessage = "No esta permitido generar OC con metodo de distribucion Predistribuida y codigos style prepack";
                    return false;
                }

                #endregion compuestos       
            }
            else
            {
                ErrorMessage = "Valores nulos en data";
                return false;
            }
            return true;
        }
        public bool DataOKIn_Codes(ref string ErrorMessage, DTOUnitario<In_Codes>.Request in_CodesRequest, int tipo)
        {
            int conteoAtributos = 0;
            string AtributoTemporal = string.Empty;
            if (in_CodesRequest.data != null)
            {
                #region unitario
                if (in_CodesRequest.data.master_code == 0)
                {
                    ErrorMessage = "Master Code Nulo";
                    return false;
                }
                else if (in_CodesRequest.data.description == string.Empty || in_CodesRequest.data.description == null)
                {
                    ErrorMessage = "Description Nulo";
                    return false;
                }
                else if (in_CodesRequest.data.packing_type == string.Empty || in_CodesRequest.data.packing_type == null)
                {
                    ErrorMessage = "packing_type Nulo";
                    return false;
                }
                else if (in_CodesRequest.data.packing_type.Trim() != "C" && in_CodesRequest.data.packing_type.Trim() != "S")
                {
                    ErrorMessage = "packing_type debe ser C o S";
                    return false;
                }
                #endregion unitario
                #region compuestos
                if (in_CodesRequest.data.line != null)
                {
                    if (in_CodesRequest.data.line.id == string.Empty || in_CodesRequest.data.line.id == null)
                    {
                        ErrorMessage = "Id Linea Nulo";
                        return false;
                    }
                }
                else
                {
                    ErrorMessage = "Linea Nulo";
                    return false;
                }
                if (in_CodesRequest.data.brand != null)
                {
                    if (in_CodesRequest.data.brand.id == string.Empty || in_CodesRequest.data.brand.id == null)
                    {
                        ErrorMessage = "Id Marca Nulo";
                        return false;
                    }
                }
                else
                {
                    ErrorMessage = "Marca Nulo";
                    return false;
                }
                if (in_CodesRequest.data.vendor != null)
                {
                    if (in_CodesRequest.data.vendor.id == string.Empty || in_CodesRequest.data.vendor.id == null)
                    {
                        ErrorMessage = "Id Vendor Nulo";
                        return false;
                    }
                }
                else
                {
                    ErrorMessage = "Vendor Nulo";
                    return false;
                }
                if (tipo == 1)
                {
                    if (in_CodesRequest.data.size_type_id != null)
                    {
                        if (in_CodesRequest.data.size_type_id.id == string.Empty || in_CodesRequest.data.size_type_id.id == null)
                        {
                            ErrorMessage = "Id Size Type Nulo";
                            return false;
                        }
                    }
                    else
                    {
                        ErrorMessage = "Size Type Nulo";
                        return false;
                    }
                    if (in_CodesRequest.data.sizes == null)
                    {
                        ErrorMessage = "Sizes Nulo";
                        return false;
                    }
                    if (in_CodesRequest.data.dim_type != null)
                    {
                        if (in_CodesRequest.data.dim_type.code == string.Empty || in_CodesRequest.data.dim_type.code == null)
                        {
                            ErrorMessage = "Id Dim Type Nulo";
                            return false;
                        }
                    }
                    else
                    {
                        ErrorMessage = "Dim Type Nulo";
                        return false;
                    }
                    if (in_CodesRequest.data.dim_values != null && in_CodesRequest.data.dim_values.Count > 0)
                    {
                        foreach (var item in in_CodesRequest.data.dim_values)
                        {
                            if (item.id == null || item.id == string.Empty)
                            {
                                ErrorMessage = "Dim_values-Id Nulo";
                                return false;
                            }
                            else if (item.name == null || item.name == string.Empty)
                            {
                                ErrorMessage = "Dim_values-Name Nulo";
                                return false;
                            }
                            else if (item.ratio.Count == 0)
                            {
                                ErrorMessage = "Dim_values-Ratio Nulo";
                                return false;
                            }
                            if (item.gs_attrs != null && item.gs_attrs.Count > 0)
                            {
                                foreach (var gs_attr in item.gs_attrs)
                                {
                                    if (gs_attr.code == null || gs_attr.code == string.Empty)
                                    {
                                        ErrorMessage = "Dim_values-gs_attr-Code Nulo";
                                        return false;
                                    }
                                    else if (gs_attr.name == null || gs_attr.name == string.Empty)
                                    {
                                        ErrorMessage = "Dim_values-gs_attr-Name Nulo";
                                        return false;
                                    }
                                    else if (gs_attr.value == null || gs_attr.name == string.Empty)
                                    {
                                        ErrorMessage = "Dim_values-gs_attr-Value Nulo";
                                        return false;
                                    }
                                    AtributoTemporal = gs_attr.code;
                                    //Validacion de dato correcto
                                    if (DatoEquivalenteCorrecto(gs_attr.code) == 1)
                                    {
                                        conteoAtributos += 1;
                                    }
                                }
                                if (conteoAtributos < 11)//
                                {
                                    ErrorMessage = "Faltan atributos que son necesarios para el proceso";
                                    return false;
                                }
                            }
                            else
                            {
                                ErrorMessage = "Dim Values-gs_attrs Nulo";
                                return false;
                            }
                        }
                    }
                    else
                    {
                        ErrorMessage = "Dim Values Nulo";
                        return false;
                    }
                }
                else
                {
                    if (in_CodesRequest.data.dim_values != null && in_CodesRequest.data.dim_values.Count > 0)
                    {
                        foreach (var item in in_CodesRequest.data.dim_values)
                        {
                            if (item.id == null || item.id == string.Empty)
                            {
                                ErrorMessage = "Dim_values-Id Nulo";
                                return false;
                            }
                            else if (item.name == null || item.name == string.Empty)
                            {
                                ErrorMessage = "Dim_values-Name Nulo";
                                return false;
                            }
                            else if (item.ratio.Count == 0)
                            {
                                ErrorMessage = "Dim_values-Ratio Nulo";
                                return false;
                            }
                            if (item.gs_attrs != null && item.gs_attrs.Count > 0)
                            {
                                foreach (var gs_attr in item.gs_attrs)
                                {
                                    if (gs_attr.code == null || gs_attr.code == string.Empty)
                                    {
                                        ErrorMessage = "Dim_values-gs_attr-Code Nulo";
                                        return false;
                                    }
                                    else if (gs_attr.name == null || gs_attr.name == string.Empty)
                                    {
                                        ErrorMessage = "Dim_values-gs_attr-Name Nulo";
                                        return false;
                                    }
                                    else if (gs_attr.value == null || gs_attr.name == string.Empty)
                                    {
                                        ErrorMessage = "Dim_values-gs_attr-Value Nulo";
                                        return false;
                                    }
                                    AtributoTemporal = gs_attr.code;
                                    //Validacion de dato correcto
                                    if (DatoEquivalenteCorrecto(gs_attr.code) == 1)
                                    {
                                        conteoAtributos += 1;
                                    }
                                }
                                if (conteoAtributos < 11)//
                                {
                                    ErrorMessage = "Faltan atributos que son necesarios para el proceso";
                                    return false;
                                }
                            }
                            else
                            {
                                ErrorMessage = "Dim Values-gs_attrs Nulo";
                                return false;
                            }
                        }
                    }
                    else
                    {
                        ErrorMessage = "Dim Values Nulo";
                        return false;
                    }
                    //if (in_CodesRequest.data.gs_attrs != null && in_CodesRequest.data.gs_attrs.Count > 0)
                    //{
                    //    foreach (var gs_attr in in_CodesRequest.data.gs_attrs)
                    //    {
                    //        if (gs_attr.code == null || gs_attr.code == string.Empty)
                    //        {
                    //            ErrorMessage = "Dim_values-gs_attr-Code Nulo";
                    //            return false;
                    //        }
                    //        else if (gs_attr.name == null || gs_attr.name == string.Empty)
                    //        {
                    //            ErrorMessage = "Dim_values-gs_attr-Name Nulo";
                    //            return false;
                    //        }
                    //        else if (gs_attr.value == null || gs_attr.value.ToString() == string.Empty)
                    //        {
                    //            ErrorMessage = "Dim_values-gs_attr-Value Nulo";
                    //            return false;
                    //        }
                    //        AtributoTemporal = gs_attr.code;
                    //        //Validacion de dato correcto
                    //        if (DatoEquivalenteCorrecto(gs_attr.code) == 1)
                    //        {
                    //            conteoAtributos += 1;
                    //        }
                    //    }
                    //    if (conteoAtributos < 11) //
                    //    {
                    //        ErrorMessage = "Faltan atributos que son necesarios para el proceso";
                    //        return false;
                    //    }
                    //}
                    //else
                    //{
                    //    ErrorMessage = "gs_attrs Nulo";
                    //    return false;
                    //}
                }
                #endregion compuestos       
            }
            else
            {
                ErrorMessage = "Valores nulos en data";
                return false;
            }
            return true;
        }
        public bool DataOKProduct_Modification(ref string ErrorMessage, ref string id, DTO<Product_modification>.Request product_ModificationRequest)
        {
            foreach (var item in product_ModificationRequest.data)
            {
                #region unitario
                if (item.parent_sku == string.Empty || item.parent_sku == null)
                {
                    ErrorMessage = "Parent Sku Nulo";
                    return false;
                }
                #endregion unitario
                #region compuestos
                if (item.attributes != null && item.attributes.Count > 0)
                {
                    foreach (var attribute in item.attributes)
                    {
                        if (attribute.code == null || attribute.code == string.Empty)
                        {
                            ErrorMessage = "Attribute-Code Nulo";
                            id = item.parent_sku;
                            return false;
                        }
                        else if (attribute.name == null || attribute.name == string.Empty)
                        {
                            ErrorMessage = "Attribute-Name Nulo";
                            id = item.parent_sku;
                            return false;
                        }
                        else if (attribute.value == null || attribute.value.ToString() == string.Empty)
                        {
                            ErrorMessage = "Attribute-Value Nulo";
                            id = item.parent_sku;
                            return false;
                        }
                    }
                }
                else
                {
                    ErrorMessage = "Attibutes Nulo";
                    id = item.parent_sku;
                    return false;
                }
                #endregion compuestos
            }
            return true;
        }

        public int DatoEquivalenteCorrecto(string atributo)
        {
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;
            int conteo = 0;

            try
            {
                oracleCommand.Parameters.Add("atributo", OracleDbType.Varchar2).Value = atributo;
                oracleCommand.Parameters.Add("conteo", OracleDbType.Int16).Direction = ParameterDirection.Output;

                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, "SP_VAL_IN_CODES_ATTRS", false))
                {
                    conteo = int.Parse(oracleCommand.Parameters["conteo"].Value.ToString());
                }
                else
                {
                    ErrorMessage = "No se realizo la conexion a la Base de datos";
                    return 0;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return 0;
            }
            return conteo;
        }

        public void Bbook_history(string tablename, string request, string response, string internaldata)
        {
            //Consultas Historicas
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;
            try
            {
                oracleCommand.Parameters.Add("bbook_table", OracleDbType.Clob).Value = tablename;
                oracleCommand.Parameters.Add("requestJson", OracleDbType.Clob).Value = request;
                oracleCommand.Parameters.Add("responseJson", OracleDbType.Clob).Value = response;
                oracleCommand.Parameters.Add("internalData", OracleDbType.Clob).Value = internaldata;
                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, "SP_BBOOK_HISTORY", false))
                {

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public string CambiarFormatoFecha(string fecha)
        {
            DateTime dt;
            if (DateTime.TryParseExact(fecha, "dd/MM/yy", CultureInfo.InvariantCulture,
                                     DateTimeStyles.None, out dt))
                dt = DateTime.ParseExact(fecha, "dd/MM/yy", CultureInfo.InvariantCulture);
            else dt = DateTime.Parse(fecha);
            return dt.ToString("yyyyMMdd");
        }

    }
}
