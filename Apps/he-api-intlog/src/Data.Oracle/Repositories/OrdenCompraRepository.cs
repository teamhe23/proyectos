using Data.Oracle.Helpers;
using Domain.Models;
using Domain.Models.Properties;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Data.Oracle.Repositories
{
    public class OrdenCompraRepository : IOrdenCompraRepository
    {
        private readonly OracleProperty oracleProperty;
        private readonly SettingProperty settingProperty;

        public OrdenCompraRepository(
            IOptions<OracleProperty> oracleProperty,
            IOptions<SettingProperty> settingProperty
            )
        {
            this.oracleProperty = oracleProperty.Value;
            this.oracleProperty.CrearCadenaConexion();
            this.settingProperty = settingProperty.Value;
        }

        public async Task<OrdenCompraResponse> Registrar_oc([FromBody] OrdenCompraRequest ocr)
        {
            using (var cn = new OracleConnection(oracleProperty.CadenaConexion))
            {
                using (var cmd = cn.CreateCommand())
                {
                    await cn.OpenAsync();

                    // agregar 
                    using (OracleTransaction tran = cn.BeginTransaction(IsolationLevel.ReadCommitted))
                    {

                        try
                        {
                            OracleDecimal IntExpPONumber;
                            OracleDecimal IntAuditNumberCab;
                            OracleDecimal IntPmgPONumber;

                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "EDSR.PKG_INTEGRACIONES_LOGISTICAS.SP_POST_ORDEN_COMPRA";
                            cmd.CommandTimeout = settingProperty.TiempoEsperaBdSegundos;

                            var oc = ocr.cabecera;
                            var ocdet = ocr.detalle;                           

                            //cabecera

                            // cmd.Parameters.Add(new OracleParameter("audit_number ", OracleDbType.Int32, ParameterDirection.Input) { Value = IntAuditNumberCab });

                            cmd.Parameters.Add(new OracleParameter("P_sistema", OracleDbType.Varchar2, ParameterDirection.Input) { Value = oc.sistema }); //"TOC" });

                            // cmd.Parameters.Add(new OracleParameter("pmg_po_number", OracleDbType.Int32, ParameterDirection.Input) { Value = IntPmgPONumber });

                            cmd.Parameters.Add(new OracleParameter("P_vendedor_number", OracleDbType.Varchar2, ParameterDirection.Input) { Value = oc.vendedor_number }); //"2038974866" });
                            cmd.Parameters.Add(new OracleParameter("P_org_lvl_number", OracleDbType.Decimal, ParameterDirection.Input) { Value =  oc.org_lvl_number }); // 501 });
                            cmd.Parameters.Add(new OracleParameter("P_dmt_code", OracleDbType.Int32, ParameterDirection.Input) { Value =oc.dmt_code }); // 2 });
                            cmd.Parameters.Add(new OracleParameter("P_rct_date", OracleDbType.Varchar2, ParameterDirection.Input) { Value = oc.rct_date }); //"20230410" });
                            cmd.Parameters.Add(new OracleParameter("P_pmg_buyer", OracleDbType.Varchar2, ParameterDirection.Input) { Value = oc.pmg_buyer }); //"Christian Robles Rojas" });
                            cmd.Parameters.Add(new OracleParameter("P_cncl_date", OracleDbType.Varchar2, ParameterDirection.Input) { Value = oc.cncl_date }); //"20230410" });

                            // cmd.Parameters.Add(new OracleParameter("ext_po_number", OracleDbType.Int32, ParameterDirection.Input) { Value = IntExpPONumber });

                            cmd.Parameters.Add(new OracleParameter("P_pmg_type_name", OracleDbType.Varchar2, ParameterDirection.Input) { Value = oc.pmg_type_name }); //"OC Nacional" });
                            cmd.Parameters.Add(new OracleParameter("P_pmg_percent", OracleDbType.Decimal, ParameterDirection.Input) { Value = oc.pmg_percent });
                            cmd.Parameters.Add(new OracleParameter("P_plazo", OracleDbType.Varchar2, ParameterDirection.Input) { Value = oc.plazo }); //"CHQ/TRF 45" });
                            cmd.Parameters.Add(new OracleParameter("P_flgcp", OracleDbType.Decimal, ParameterDirection.Input) { Value = oc.flgcp });
                            cmd.Parameters.Add(new OracleParameter("P_pmg_lc_number", OracleDbType.Varchar2, ParameterDirection.Input) { Value =oc.pmg_lc_number }); //"1"});
                            cmd.Parameters.Add(new OracleParameter("P_flg_req_ins", OracleDbType.Varchar2, ParameterDirection.Input) { Value =  oc.flg_req_ins }); //"N" }); 
                            cmd.Parameters.Add(new OracleParameter("P_shipping_date", OracleDbType.Varchar2, ParameterDirection.Input) { Value =  oc.shipping_date }); //"20230414" }); 
                            cmd.Parameters.Add(new OracleParameter("P_id_puerto", OracleDbType.Decimal, ParameterDirection.Input) { Value = oc.id_puerto });
                            cmd.Parameters.Add(new OracleParameter("P_id_incoterm", OracleDbType.Varchar2, ParameterDirection.Input) { Value =   oc.id_incoterm }); //"-1" }); 
                            cmd.Parameters.Add(new OracleParameter("P_contenedor_20", OracleDbType.Decimal, ParameterDirection.Input) { Value = oc.contenedor_20 });
                            cmd.Parameters.Add(new OracleParameter("P_contenedor_40", OracleDbType.Decimal, ParameterDirection.Input) { Value = oc.contenedor_40 });
                            cmd.Parameters.Add(new OracleParameter("P_contenedor_hc", OracleDbType.Decimal, ParameterDirection.Input) { Value = oc.contenedor_hc });
                            cmd.Parameters.Add(new OracleParameter("P_contenedor_lcl", OracleDbType.Decimal, ParameterDirection.Input) { Value = oc.contenedor_lcl });


                            
                            cmd.Parameters.Add("P_IntExpPONumber", OracleDbType.Decimal, ParameterDirection.Output);
                            cmd.Parameters.Add("P_IntAuditNumberCab", OracleDbType.Decimal, ParameterDirection.Output);
                            cmd.Parameters.Add("P_IntPmgPONumber", OracleDbType.Decimal, ParameterDirection.Output);
                            await cmd.ExecuteNonQueryAsync();

                            IntExpPONumber = ((OracleDecimal)cmd.Parameters["P_IntExpPONumber"].Value);
                            IntAuditNumberCab = ((OracleDecimal)cmd.Parameters["P_IntAuditNumberCab"].Value);
                            IntPmgPONumber = ((OracleDecimal)cmd.Parameters["P_IntPmgPONumber"].Value);
                        
                            cmd.Parameters.Clear();

                            cmd.CommandText = "EDSR.PKG_INTEGRACIONES_LOGISTICAS.SP_POST_ORDEN_COMPRA_DETALLE";
                            //detalle
                            int i=0;
                            foreach (var oc_item in ocdet)
                            {
                                i++;
                                cmd.Parameters.Add(new OracleParameter("P_pmg_line_number", OracleDbType.Decimal, ParameterDirection.Input) { Value = i });
                                cmd.Parameters.Add(new OracleParameter("P_prd_lvl_number", OracleDbType.Varchar2, ParameterDirection.Input) { Value =  oc_item.prd_lvl_number }); //"137471" }); 
                                cmd.Parameters.Add(new OracleParameter("P_org_lvl_number", OracleDbType.Decimal, ParameterDirection.Input) { Value = oc_item.org_lvl_number }); //501 }); 
                                cmd.Parameters.Add(new OracleParameter("P_dist_sell_qty", OracleDbType.Decimal, ParameterDirection.Input) { Value = oc_item.dist_sell_qty }); //24 }); 
                                cmd.Parameters.Add(new OracleParameter("P_dmt_code", OracleDbType.Int32, ParameterDirection.Input) { Value = oc_item.dmt_code }); // 2 });
                                cmd.Parameters.Add(new OracleParameter("P_ext_po_number", OracleDbType.Varchar2, ParameterDirection.Input) { Value = IntExpPONumber });
                                cmd.Parameters.Add(new OracleParameter("P_prd_lvl_id", OracleDbType.Int32, ParameterDirection.Input) { Value = oc_item.prd_lvl_id });
                                cmd.Parameters.Add(new OracleParameter("P_cod_stl", OracleDbType.Varchar2, ParameterDirection.Input) { Value =  oc_item.cod_stl }); //"137471" }); 
                                cmd.Parameters.Add(new OracleParameter("P_audit_number_cab", OracleDbType.Decimal, ParameterDirection.Input) { Value = IntAuditNumberCab });
                                cmd.Parameters.Add(new OracleParameter("P_pmg_sell_cost", OracleDbType.Decimal, ParameterDirection.Input) { Value = oc_item.pmg_sell_cost }); //5.94}); 

                                await cmd.ExecuteNonQueryAsync();                               
                                cmd.Parameters.Clear();
                            }

                            cmd.CommandText = "EDSR.TP_PKG_ARCHIVOS_BBR.sp_inserta_sol_oc";
                            cmd.Parameters.Add(new OracleParameter("SISTEMA ", OracleDbType.Varchar2, ParameterDirection.Input) { Value = "TOC" });
                            cmd.Parameters.Add(new OracleParameter("EXECGENOC ", OracleDbType.Int32, ParameterDirection.Input) { Value = 1 });
                            cmd.Parameters.Add(new OracleParameter("AUTOCOMMIT ", OracleDbType.Varchar2, ParameterDirection.Input) { Value = "F" });
                            cmd.Parameters.Add(new OracleParameter("PMGPONUMBER ", OracleDbType.Int32, ParameterDirection.Input) { Value = IntPmgPONumber });
                            await cmd.ExecuteNonQueryAsync();
                            cmd.Parameters.Clear();

                            tran.Commit(); //confirmo que todo esta bien
                            await cn.CloseAsync();

                            
                            OrdenCompraResponse response = new OrdenCompraResponse();
                            response.pmg_po_number = (decimal)(IntPmgPONumber);                            
                            return response;
                        }                        
                        catch (OracleException ex)
                        {
                            tran.Rollback();                            
                            throw new ExceptionInternal(ex.Message);                                                   
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            throw new ExceptionInternal(ex.Message);                           
                        }
                    }                                       
                }
            }
        }


        public async Task<List<BuscarOC>> Buscar_oc(string finicial, string ffinal,
                                                    int? vpc_tech_key,
                                                    int? pmg_po_number
                                                     )
        {
            using (var cn = new OracleConnection(oracleProperty.CadenaConexion))
            {

                using (var cmd = cn.CreateCommand())
                {
                    await cn.OpenAsync();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "EDSR.PKG_INTEGRACIONES_LOGISTICAS.SP_GET_ORDEN_COMPRA";
                    cmd.CommandTimeout = settingProperty.TiempoEsperaBdSegundos;

                    DateTime fechaInicioFormatted = DateTime.ParseExact(finicial, "ddMMyyyy", CultureInfo.InvariantCulture);
                    DateTime fechaFinFormatted = DateTime.ParseExact(ffinal, "ddMMyyyy", CultureInfo.InvariantCulture);

                    cmd.Parameters.Add(new OracleParameter("P_FECHAINICIO", OracleDbType.Varchar2, ParameterDirection.Input) { Value = fechaInicioFormatted.ToString("ddMMyyyy") });
                    cmd.Parameters.Add(new OracleParameter("P_FECHAFIN", OracleDbType.Varchar2, ParameterDirection.Input) { Value = fechaFinFormatted.ToString("ddMMyyyy") });
                    cmd.Parameters.Add(new OracleParameter("P_VPC_TECH_KEY", OracleDbType.Int64, ParameterDirection.Input) { Value = vpc_tech_key });
                    cmd.Parameters.Add(new OracleParameter("P_PMG_PO_NUMBER", OracleDbType.Int64, ParameterDirection.Input) { Value = pmg_po_number });
                    cmd.Parameters.Add("P_ORDENCOMPRA", OracleDbType.RefCursor, ParameterDirection.Output);

                    await cmd.ExecuteNonQueryAsync();
                    var buscar = OracleConvert.CursorToModel<BuscarOC>(cmd.Parameters["P_ORDENCOMPRA"]);

                    await cn.CloseAsync();
                    return buscar;
                }
            }
        }
    }
}
