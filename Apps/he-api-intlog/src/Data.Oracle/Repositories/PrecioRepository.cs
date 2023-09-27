using Data.Oracle.Helpers;
using Domain.Models;
using Domain.Models.Filters;
using Domain.Models.Properties;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Oracle.Repositories
{
    public class PrecioRepository : IPrecioRepository
    {
        private readonly OracleProperty oracleProperty;
        private readonly SettingProperty settingProperty;

        public PrecioRepository(
            IOptions<OracleProperty> oracleProperty,
            IOptions<SettingProperty> settingProperty
            )
        {
            this.oracleProperty = oracleProperty.Value;
            this.oracleProperty.CrearCadenaConexion();
            this.settingProperty = settingProperty.Value;
        }

        public async Task<PrecioProgramadoResponse> GuardarProgramado(PrecioProgramadoRequest precioProgramadoRequest)
        {
            var response = new PrecioProgramadoResponse();

            using (var cn = new OracleConnection(oracleProperty.CadenaConexion))
            {
                using (var cmd = cn.CreateCommand())
                {
                    await cn.OpenAsync();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "EDSR.PKG_INTEGRACIONES_LOGISTICAS.SP_PROC_PRECIO_PROGRAMADO";
                    cmd.CommandTimeout = settingProperty.TiempoEsperaBdSegundos;

                    cmd.Parameters.Add("P_cap_chain_id", OracleDbType.Int64).Value = precioProgramadoRequest.cabecera.cap_chain_id;
                    cmd.Parameters.Add("P_prc_type", OracleDbType.Int64).Value = precioProgramadoRequest.cabecera.prc_type;
                    cmd.Parameters.Add("P_fecha_inicio", OracleDbType.Varchar2).Value = precioProgramadoRequest.cabecera.fecha_inicio;
                    cmd.Parameters.Add("P_descripcion", OracleDbType.Varchar2).Value = precioProgramadoRequest.cabecera.descripcion;
                    cmd.Parameters.Add("P_Usuario", OracleDbType.Varchar2).Value = precioProgramadoRequest.cabecera.usuario;
                    cmd.Parameters.Add("P_UserHost", OracleDbType.Varchar2).Value = precioProgramadoRequest.cabecera.userhost;

                    OracleParameter pSucursal = new OracleParameter();
                    pSucursal.ParameterName = "P_ORG_LVL_CHILD";
                    pSucursal.Direction = ParameterDirection.Input;
                    pSucursal.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                    pSucursal.OracleDbType = OracleDbType.Int64;
                    pSucursal.Size = precioProgramadoRequest.productos.Count;
                    pSucursal.Value = precioProgramadoRequest.productos.Select(item => item.id_sucursal).ToArray();
                    cmd.Parameters.Add(pSucursal);

                    OracleParameter pProducto = new OracleParameter();
                    pProducto.ParameterName = "P_PRD_LVL_CHILD";
                    pProducto.Direction = ParameterDirection.Input;
                    pProducto.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                    pProducto.OracleDbType = OracleDbType.Int64;
                    pProducto.Size = precioProgramadoRequest.productos.Count;
                    pProducto.Value = precioProgramadoRequest.productos.Select(item => item.prd_lvl_child).ToArray();
                    cmd.Parameters.Add(pProducto);

                    OracleParameter pPrecio = new OracleParameter();
                    pPrecio.ParameterName = "P_Precio";
                    pPrecio.Direction = ParameterDirection.Input;
                    pPrecio.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                    pPrecio.OracleDbType = OracleDbType.Decimal;
                    pPrecio.Size = precioProgramadoRequest.productos.Count;
                    pPrecio.Value = precioProgramadoRequest.productos.Select(item => item.precio).ToArray();
                    cmd.Parameters.Add(pPrecio);

                    OracleParameter pUnidMed = new OracleParameter();
                    pUnidMed.ParameterName = "P_cod_umv";
                    pUnidMed.Direction = ParameterDirection.Input;
                    pUnidMed.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                    pUnidMed.OracleDbType = OracleDbType.Varchar2;
                    pUnidMed.Size = precioProgramadoRequest.productos.Count;
                    pUnidMed.Value = precioProgramadoRequest.productos.Select(item => item.cod_umv).ToArray();
                    cmd.Parameters.Add(pUnidMed);

                    OracleParameter pQtyIniRan1 = new OracleParameter();
                    pQtyIniRan1.ParameterName = "P_qty_ini_ran1";
                    pQtyIniRan1.Direction = ParameterDirection.Input;
                    pQtyIniRan1.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                    pQtyIniRan1.OracleDbType = OracleDbType.Decimal;
                    pQtyIniRan1.Size = precioProgramadoRequest.productos.Count;
                    pQtyIniRan1.Value = precioProgramadoRequest.productos.Select(item => item.qty_ini_ran1).ToArray();
                    cmd.Parameters.Add(pQtyIniRan1);

                    OracleParameter pPctRan1 = new OracleParameter();
                    pPctRan1.ParameterName = "P_pct_ran1";
                    pPctRan1.Direction = ParameterDirection.Input;
                    pPctRan1.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                    pPctRan1.OracleDbType = OracleDbType.Decimal;
                    pPctRan1.Size = precioProgramadoRequest.productos.Count;
                    pPctRan1.Value = precioProgramadoRequest.productos.Select(item => item.pct_ran1).ToArray();
                    cmd.Parameters.Add(pPctRan1);

                    OracleParameter pPrcRan1 = new OracleParameter();
                    pPrcRan1.ParameterName = "P_prc_ran1";
                    pPrcRan1.Direction = ParameterDirection.Input;
                    pPrcRan1.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                    pPrcRan1.OracleDbType = OracleDbType.Decimal;
                    pPrcRan1.Size = precioProgramadoRequest.productos.Count;
                    pPrcRan1.Value = precioProgramadoRequest.productos.Select(item => item.prc_ran1).ToArray();
                    cmd.Parameters.Add(pPrcRan1);

                    OracleParameter pQtyIniRan2 = new OracleParameter();
                    pQtyIniRan2.ParameterName = "P_qty_ini_ran2";
                    pQtyIniRan2.Direction = ParameterDirection.Input;
                    pQtyIniRan2.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                    pQtyIniRan2.OracleDbType = OracleDbType.Decimal;
                    pQtyIniRan2.Size = precioProgramadoRequest.productos.Count;
                    pQtyIniRan2.Value = precioProgramadoRequest.productos.Select(item => item.qty_ini_ran2).ToArray();
                    cmd.Parameters.Add(pQtyIniRan2);

                    OracleParameter pPctRan2 = new OracleParameter();
                    pPctRan2.ParameterName = "P_pct_ran2";
                    pPctRan2.Direction = ParameterDirection.Input;
                    pPctRan2.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                    pPctRan2.OracleDbType = OracleDbType.Decimal;
                    pPctRan2.Size = precioProgramadoRequest.productos.Count;
                    pPctRan2.Value = precioProgramadoRequest.productos.Select(item => item.pct_ran2).ToArray();
                    cmd.Parameters.Add(pPctRan2);

                    OracleParameter pPrcRan2 = new OracleParameter();
                    pPrcRan2.ParameterName = "P_prc_ran2";
                    pPrcRan2.Direction = ParameterDirection.Input;
                    pPrcRan2.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                    pPrcRan2.OracleDbType = OracleDbType.Decimal;
                    pPrcRan2.Size = precioProgramadoRequest.productos.Count;
                    pPrcRan2.Value = precioProgramadoRequest.productos.Select(item => item.prc_ran2).ToArray();
                    cmd.Parameters.Add(pPrcRan2);

                    OracleParameter pIndRecaTriprc = new OracleParameter();
                    pIndRecaTriprc.ParameterName = "P_ind_reca_triprc";
                    pIndRecaTriprc.Direction = ParameterDirection.Input;
                    pIndRecaTriprc.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                    pIndRecaTriprc.OracleDbType = OracleDbType.Int64;
                    pIndRecaTriprc.Size = precioProgramadoRequest.productos.Count;
                    pIndRecaTriprc.Value = precioProgramadoRequest.productos.Select(item => item.ind_reca_triprc ? 1 : 0).ToArray();
                    cmd.Parameters.Add(pIndRecaTriprc);

                    OracleParameter pIndCalcTriprc = new OracleParameter();
                    pIndCalcTriprc.ParameterName = "P_ind_calc_triprc";
                    pIndCalcTriprc.Direction = ParameterDirection.Input;
                    pIndCalcTriprc.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                    pIndCalcTriprc.OracleDbType = OracleDbType.Int64;
                    pIndCalcTriprc.Size = precioProgramadoRequest.productos.Count;
                    pIndCalcTriprc.Value = precioProgramadoRequest.productos.Select(item => item.ind_calc_triprc ? 1 : 0).ToArray();
                    cmd.Parameters.Add(pIndCalcTriprc);

                    cmd.Parameters.Add("NRO_EVENTO", OracleDbType.Int64, ParameterDirection.Output);
                    cmd.Parameters.Add("ERRORES", OracleDbType.RefCursor, ParameterDirection.Output);

                    await cmd.ExecuteNonQueryAsync();

                    OracleDecimal nro_evento = ((OracleDecimal)cmd.Parameters["NRO_EVENTO"].Value);
                    response.num_eve_prc = (Int64)nro_evento;
                    response.errores = OracleConvert.CursorToModel<PrecioProgramadoResponseError>(cmd.Parameters["ERRORES"]);

                    await cn.CloseAsync();
                }
            }
            return response;
        }

        public async Task<List<PrecioProgramadoBuscar>> ListarProgramado([FromQuery] PrecioProgramadoFilterGet filtro)
        {
            using (var cn = new OracleConnection(oracleProperty.CadenaConexion))
            {
                using (var cmd = cn.CreateCommand())
                {
                    await cn.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "edsr.PKG_INTEGRACIONES_LOGISTICAS.SP_SEL_PRECIO_PROGRAMADO";
                    cmd.CommandTimeout = settingProperty.TiempoEsperaBdSegundos;
                    cmd.Parameters.Add("pSucursal", OracleDbType.Int64).Value = filtro.org_lvl_child;
                    cmd.Parameters.Add("pProducto", OracleDbType.Int64).Value = filtro.prd_lvl_child;
                    cmd.Parameters.Add("pFecIni", OracleDbType.Varchar2).Value = filtro.fecha_ini;
                    cmd.Parameters.Add("pFecFin", OracleDbType.Varchar2).Value = filtro.fecha_fin;
                    cmd.Parameters.Add("PRECIOS", OracleDbType.RefCursor, ParameterDirection.Output);

                    await cmd.ExecuteNonQueryAsync();

                    var precioenprogramado = OracleConvert.CursorToModel<PrecioProgramadoBuscar>(cmd.Parameters["PRECIOS"]);

                    await cn.CloseAsync();

                    return precioenprogramado;

                }
            }
        }
    }
}
