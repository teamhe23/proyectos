using Data.Oracle.Helpers;
using Domain.Models;
using Domain.Models.Properties;
using Domain.Repositories;
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
    public class SucursalesRepository : ISucursalesRepository
    {
        private readonly OracleProperty oracleProperty;
        private readonly SettingProperty settingProperty;

        public SucursalesRepository(
            IOptions<OracleProperty> oracleProperty,
            IOptions<SettingProperty> settingProperty
        )
        {
            this.oracleProperty = oracleProperty.Value;
            this.oracleProperty.CrearCadenaConexion();
            this.settingProperty = settingProperty.Value;
        }

        public async Task<List<Sucursales>> ListarSucursales(Int64 pnivel, Int64? pparent)
        {
            using (var cn = new OracleConnection(oracleProperty.CadenaConexion))
            {
                using (var cmd = cn.CreateCommand())
                {
                    await cn.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "EDSR.PKG_INTEGRACIONES_LOGISTICAS.SP_GET_SUCURSALES";
                    cmd.CommandTimeout = settingProperty.TiempoEsperaBdSegundos;

                    cmd.Parameters.Add(new OracleParameter("P_NIVEL", OracleDbType.Int64, ParameterDirection.Input) { Value = pnivel });
                    cmd.Parameters.Add(new OracleParameter("P_PARENT", OracleDbType.Int64, ParameterDirection.Input) { Value = pparent });
                    cmd.Parameters.Add("SUCURSALES", OracleDbType.RefCursor, ParameterDirection.Output);
                    await cmd.ExecuteNonQueryAsync();

                    var Sucursales = OracleConvert.CursorToModel<Sucursales>(cmd.Parameters["SUCURSALES"]);

                    await cn.CloseAsync();
                    return Sucursales;
                }
            }
        }


    }
}
