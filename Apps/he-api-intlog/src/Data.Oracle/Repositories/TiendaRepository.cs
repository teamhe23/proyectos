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
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Data.Oracle.Repositories
{
    public class TiendaRepository : ITiendaRepository
    {
        private readonly OracleProperty oracleProperty;
        private readonly SettingProperty settingProperty;

        public TiendaRepository(
           IOptions<OracleProperty> oracleProperty,
           IOptions<SettingProperty> settingProperty
           )
        {
            this.oracleProperty = oracleProperty.Value;
            this.oracleProperty.CrearCadenaConexion();
            this.settingProperty = settingProperty.Value;
        }
 
        public async Task<List<Tienda>>ListarTiendas(int solo_cd)
        {
            using (var cn = new OracleConnection(oracleProperty.CadenaConexion))
            {
                List<Tienda> tienda = new List<Tienda>();

                using (var cmd = cn.CreateCommand())
                {
                    await cn.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "edsr.PKG_INTEGRACIONES_LOGISTICAS.SP_SEL_TIENDA";
                    cmd.CommandTimeout = settingProperty.TiempoEsperaBdSegundos;
                    cmd.Parameters.Add(new OracleParameter("P_SOLO_CD", OracleDbType.Int32, ParameterDirection.Input) { Value = solo_cd });
                    cmd.Parameters.Add("TIENDAENTREGA", OracleDbType.RefCursor, ParameterDirection.Output);
                    await cmd.ExecuteNonQueryAsync();

                    OracleDataReader dataReader = ((OracleRefCursor)cmd.Parameters["TIENDAENTREGA"].Value).GetDataReader();

                    if (dataReader != null && dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            tienda.Add(new Tienda()
                            {
                                org_lvl_child = Int32.Parse(dataReader["ORG_LVL_CHILD"].ToString()),
                                codigo = Int32.Parse(dataReader["CODIGO"].ToString()),
                                descripcion = dataReader["DESCRIPCION"].ToString(),
                                org_is_store = dataReader["ORG_IS_STORE"].ToString()                                
                            });

                        }
                    }

                    await cn.CloseAsync();
                    return tienda;
                }
            }
        }

    }
}
