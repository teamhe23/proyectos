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
using System.Text;
using System.Threading.Tasks;

namespace Data.Oracle.Repositories
{
    public class FechaPMMRepository : IFechaPMMRepository
    {
        private readonly OracleProperty oracleProperty;
        private readonly SettingProperty settingProperty;

        public FechaPMMRepository(
            IOptions<OracleProperty> oracleProperty,
            IOptions<SettingProperty> settingProperty
            )
        {
            this.oracleProperty = oracleProperty.Value;
            this.oracleProperty.CrearCadenaConexion();
            this.settingProperty = settingProperty.Value;
        }

        public async Task<Fecha> ObtenerFecha()
        {
            using (var cn = new OracleConnection(oracleProperty.CadenaConexion))
            {
                using (var cmd = cn.CreateCommand())
                {
                    await cn.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "edsr.PKG_INTEGRACIONES_LOGISTICAS.SP_GET_FECHA_PMM";
                    cmd.CommandTimeout = settingProperty.TiempoEsperaBdSegundos;
                    cmd.Parameters.Add("P_FECHA", OracleDbType.Date, ParameterDirection.Output);
                    await cmd.ExecuteNonQueryAsync();

                    var oracleDate = ((OracleDate)cmd.Parameters["P_FECHA"].Value);
                    var fechaPMM = new Fecha { FechaPMM = oracleDate.Value};
                    await cn.CloseAsync();

                    return fechaPMM;

                }
            }
        }

    }    
}
