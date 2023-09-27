using Data.Oracle.Helpers;
using Domain.Models;
using Domain.Models.Filters;
using Domain.Models.Properties;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Oracle.Repositories
{
    public class PrecioEnLineaRepository : IPrecioEnLineaRepository
    {
        private readonly OracleProperty oracleProperty;
        private readonly SettingProperty settingProperty;

        public PrecioEnLineaRepository(
            IOptions<OracleProperty> oracleProperty,
            IOptions<SettingProperty> settingProperty
            )
        {
            this.oracleProperty = oracleProperty.Value;
            this.oracleProperty.CrearCadenaConexion();
            this.settingProperty = settingProperty.Value;
        }


        public async Task<PrecioEnLineaResponse> registrar_precio_en_linea([FromBody] PrecioEnLineaRequest pe)
        {
            var response = new PrecioEnLineaResponse();

            using (var cn = new OracleConnection(oracleProperty.CadenaConexion))
            {
                using (var cmd = cn.CreateCommand())
                {
                    await cn.OpenAsync();

                    using (OracleTransaction tran = cn.BeginTransaction(IsolationLevel.ReadCommitted))
                    {
                        try
                        {
                            var plinea = pe.cabecera;
                            var plineadet = pe.detalle;

                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "EDSR.PKG_INTEGRACIONES_LOGISTICAS.SP_GET_PRODUCTOS_TE";
                            cmd.CommandTimeout = settingProperty.TiempoEsperaBdSegundos;

                            ///

                            cmd.CommandText = "EDSR.PKG_INTEGRACIONES_LOGISTICAS.SP_INS_PRECIO_EN_LINEA";

                            ///

                            tran.Commit();                           

                            return response;
                        }
                        catch (OracleException ex)
                        {
                            tran.Rollback();
                            response.errores.Add(new ExceptionInternal(ex.Message).Message);
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            response.errores.Add(ex.Message);
                        }
                        finally
                        {
                            await cn.CloseAsync();

                        }
                        return null;
                    }
                }
            }
        }




        public async Task<List<BuscarPrecioEnLinea>> ListarPrecioEnLinea(PrecioEnLineaFilterGet filtro)
        {
            using (var cn = new OracleConnection(oracleProperty.CadenaConexion))
            {
                using (var cmd = cn.CreateCommand())
                {
                    await cn.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "edsr.PKG_INTEGRACIONES_LOGISTICAS.SP_GET_REPORTE_PRECIO_EN_LINEA";
                    cmd.CommandTimeout = settingProperty.TiempoEsperaBdSegundos;

                    cmd.Parameters.Add("pSucursal", OracleDbType.Int64).Value = filtro.sucursal;
                    cmd.Parameters.Add("pProducto", OracleDbType.Int64).Value = filtro.producto;
                    cmd.Parameters.Add("pFecIni", OracleDbType.Varchar2).Value = filtro.fechainicio;
                    cmd.Parameters.Add("pFecFin", OracleDbType.Varchar2).Value = filtro.fechafin;
                    cmd.Parameters.Add("PRECIOENLINEA", OracleDbType.RefCursor, ParameterDirection.Output);

                    await cmd.ExecuteNonQueryAsync();

                    var precioenlinea = OracleConvert.CursorToModel<BuscarPrecioEnLinea>(cmd.Parameters["PRECIOENLINEA"]);

                    await cn.CloseAsync();

                    return precioenlinea;

                }
            }
        }
    }    
}
