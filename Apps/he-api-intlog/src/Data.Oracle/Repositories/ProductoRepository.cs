using Data.Oracle.Helpers;
using Domain.Models;
using Domain.Models.Properties;
using Domain.Repositories;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Data.Oracle.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly OracleProperty oracleProperty;
        private readonly SettingProperty settingProperty;

        public ProductoRepository(
            IOptions<OracleProperty> oracleProperty,
            IOptions<SettingProperty> settingProperty
            )
        {
            this.oracleProperty = oracleProperty.Value;
            this.oracleProperty.CrearCadenaConexion();
            this.settingProperty = settingProperty.Value;
        }


        public async Task<Producto> buscar_producto_oc(int metodo_distribucion, int vpc_tech_key, int org_lvl_child, string prd_lvl_number)
        {
            using (var cn = new OracleConnection(oracleProperty.CadenaConexion))
            {
                Producto producto = null;

                using (var cmd = cn.CreateCommand())
                {
                    await cn.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "edsr.PKG_INTEGRACIONES_LOGISTICAS.SP_GET_PRODUCTO_OC";
                    cmd.CommandTimeout = settingProperty.TiempoEsperaBdSegundos;
                    cmd.Parameters.Add(new OracleParameter("p_metodo_distribucion ", OracleDbType.Int32, ParameterDirection.Input) { Value = metodo_distribucion });
                    cmd.Parameters.Add(new OracleParameter("p_vpc_tech_key", OracleDbType.Int32, ParameterDirection.Input) { Value = vpc_tech_key });
                    cmd.Parameters.Add(new OracleParameter("p_org_lvl_child", OracleDbType.Int32, ParameterDirection.Input) { Value = org_lvl_child });
                    cmd.Parameters.Add(new OracleParameter("p_prd_lvl_number", OracleDbType.Varchar2, ParameterDirection.Input) { Value = prd_lvl_number });
                    cmd.Parameters.Add("PRODUCTO", OracleDbType.RefCursor, ParameterDirection.Output);

                    await cmd.ExecuteNonQueryAsync();

                    OracleDataReader dataReader = ((OracleRefCursor)cmd.Parameters["PRODUCTO"].Value).GetDataReader();

                    if (dataReader != null && dataReader.HasRows)
                    {
                        await dataReader.ReadAsync();

                        producto = new Producto()
                        {
                            prd_lvl_child = Int32.Parse(dataReader["prd_lvl_child"].ToString()),
                            prd_lvl_number = dataReader["prd_lvl_number"].ToString(),
                            prd_name_full = dataReader["prd_name_full"].ToString(),
                            costo_case_pack = decimal.Parse(dataReader["COSTO_CASE_PACK"].ToString()),
                            master_pack = Int32.Parse(dataReader["MASTER_PACK"].ToString()),
                            cantidad_minima = Int32.Parse(dataReader["CANTIDAD_MINIMA"].ToString())                          
                        };
                    }

                    await cn.CloseAsync();
                    return producto;

                }
            }
        }

        public async Task<ProductoPrecio?> ObtenerProductoPrecio(string codigo)
        {
            ProductoPrecio? productoPrecio = null;

            using (var cn = new OracleConnection(oracleProperty.CadenaConexion))
            {
                using (var cmd = cn.CreateCommand())
                {
                    await cn.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "edsr.PKG_INTEGRACIONES_LOGISTICAS.SP_GET_PRODUCTOS_PRECIO";
                    cmd.CommandTimeout = settingProperty.TiempoEsperaBdSegundos;
                    cmd.Parameters.Add(new OracleParameter("P_SKU", OracleDbType.Varchar2, ParameterDirection.Input) { Value = codigo });
                    cmd.Parameters.Add("PRODUCTO", OracleDbType.RefCursor, ParameterDirection.Output);

                    await cmd.ExecuteNonQueryAsync();

                    var productos = OracleConvert.CursorToModel<ProductoPrecio>(cmd.Parameters["PRODUCTO"]);

                    if (productos.Any())
                    {
                        productoPrecio = productos.FirstOrDefault();
                    }
                    await cn.CloseAsync();
                }
            }
            return productoPrecio;
        }
    }
}
