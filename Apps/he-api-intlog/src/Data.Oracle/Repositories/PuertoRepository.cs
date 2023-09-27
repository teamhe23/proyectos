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
    public class PuertoRepository : IPuertoRepository
    {
        private readonly OracleProperty oracleProperty;
        private readonly SettingProperty settingProperty;

        public PuertoRepository(
            IOptions<OracleProperty> oracleProperty,
            IOptions<SettingProperty> settingProperty
            )
        {
            this.oracleProperty = oracleProperty.Value;
            this.oracleProperty.CrearCadenaConexion();
            this.settingProperty = settingProperty.Value;
        }

        public async Task<List<Puerto>> ListarPuerto(int psucursal)
        {
            using (var cn = new OracleConnection(oracleProperty.CadenaConexion))
            {
                List<Puerto> puerto = new List<Puerto>();

                using (var cmd = cn.CreateCommand())
                {
                   
                    await cn.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "edsr.PKG_INTEGRACIONES_LOGISTICAS.SP_SEL_PUERTO";
                    cmd.CommandTimeout = settingProperty.TiempoEsperaBdSegundos;
                    cmd.Parameters.Add(new OracleParameter("P_SUCURSAL", OracleDbType.Int32, ParameterDirection.Input) { Value = psucursal });
                    cmd.Parameters.Add("P_PUERTO", OracleDbType.RefCursor, ParameterDirection.Output);
                    await cmd.ExecuteNonQueryAsync();

                    OracleDataReader dataReader = ((OracleRefCursor)cmd.Parameters["P_PUERTO"].Value).GetDataReader();

                    if (dataReader != null && dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            puerto.Add(new Puerto()
                            {
                                id = Int32.Parse(dataReader["ID"].ToString()),
                                descripcion = dataReader["DESCRIP"].ToString(),
                                dias = Int32.Parse(dataReader["DIAS"].ToString())
                            });

                        }
                    }                   

                    await cn.CloseAsync();
                    return puerto;
                }
            }
        }


    }
}
