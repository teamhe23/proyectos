using Data.Oracle.Helpers;
using Domain.Models;
using Domain.Models.Properties;
using Domain.Repositories.Oracle;
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
    public class LogsRepository : ILogsRepository
    {
        private readonly OracleProperties _oracleProperties;
        public LogsRepository(IOptions<OracleProperties> oracleProperties)
        {
            _oracleProperties = oracleProperties.Value;
            _oracleProperties.SetConnection();
        }
        public async Task Post(Logs ologs)
        {
            using (var cn = new OracleConnection(_oracleProperties.stringConexion))
            {
                using (var cmd = cn.CreateCommand())
                {
                    cn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "TIENDAS_ADM.WMS_INTEGRACION_API.SP_INS_WMS_LOG_INTEGRACION";
                    cmd.CommandTimeout = 60000;
                    cmd.Parameters.Add("P_ID_TIPO", OracleDbType.Varchar2, ParameterDirection.Input).Value = ologs.ID_TIPO;
                    cmd.Parameters.Add("P_IDENTIFICADOR", OracleDbType.Varchar2, ParameterDirection.Input).Value = ologs.IDENTIFICADOR;
                    cmd.Parameters.Add("P_MENSAJE", OracleDbType.Varchar2, ParameterDirection.Input).Value = ologs.MENSAJE;
                    cmd.Parameters.Add("P_TRAMA", OracleDbType.Clob, ParameterDirection.Input).Value = ologs.TRAMA;

                    await cmd.ExecuteNonQueryAsync();

                    return;
                }
            }
        }
    }
}
