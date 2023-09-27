using Data.Oracle.Helpers;
using Domain.Models;
using Domain.Models.Properties;
using Domain.Repositories.Oracle;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Data.Oracle.Repositories
{
    public class TipoIntegracionRepository : ITipoIntegracionRepository
    {
        private readonly OracleProperties _oracleProperties;
        public TipoIntegracionRepository(IOptions<OracleProperties> oracleProperties)
        {
            _oracleProperties = oracleProperties.Value;
            _oracleProperties.SetConnection();
        }
        
        public async Task<List<TipoIntegracion>> get(Int64? IdProceso)
        {
            using (var cn = new OracleConnection(_oracleProperties.stringConexion))
            {
                using (var cmd = cn.CreateCommand())
                {
                    cn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "TIENDAS_ADM.WMS_INTEGRACION_API.SP_GET_WMS_TIPO_INTEGRACION";
                    cmd.CommandTimeout = 60000;
                    cmd.Parameters.Add("P_ID_TIPO", OracleDbType.Int64, ParameterDirection.Input).Value = IdProceso;
                    cmd.Parameters.Add("TIPO_INTEGRACION", OracleDbType.RefCursor, ParameterDirection.Output);

                    await cmd.ExecuteNonQueryAsync();

                    return OracleConvert.CursorToModel<TipoIntegracion>(cmd.Parameters["TIPO_INTEGRACION"]);
         
                }
            }
        }
    }
}
