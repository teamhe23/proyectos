using Domain.Models.Properties;
using Domain.Models;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using Domain.Repositories;

namespace Data.Oracle.Repositories
{
    public class ModeloRequestRepository : IModeloRequestRepository
    {
        private readonly OracleProperty _oracleProperty;
        private readonly Settings _settings;

        public ModeloRequestRepository(IOptions<OracleProperty> oracleProperty,
                                       IOptions<Settings> settings)
        {
            _oracleProperty = oracleProperty.Value;
            _oracleProperty.SetConnection();
            _settings = settings.Value;
        }

        public async Task<Int64> InsModeloRequest(ModeloRequest modeloRequest)
        {
            using (var cn = new OracleConnection(_oracleProperty.stringConexion))
            {
                using (var cmd = new OracleCommand())
                {
                    await cn.OpenAsync();

                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "EDSR.PKG_WMS_GENERAL.SP_INS_WMS_MODELO_REQUEST";
                    cmd.CommandTimeout = _settings.TiempoEsperaSegundos;

                    cmd.Parameters.Add("P_ID_MODELO", OracleDbType.Int64, ParameterDirection.Output);
                    cmd.Parameters.Add("P_ID_TIPO", OracleDbType.Int16).Value = modeloRequest.IdTipo;
                    cmd.Parameters.Add("P_MODELO", OracleDbType.Clob).Value = modeloRequest.Modelo;
                    cmd.Parameters.Add("P_MESSAGE_ID", OracleDbType.Varchar2).Value = modeloRequest.MessageId;

                    await cmd.ExecuteNonQueryAsync();

                    var orDecimal = OracleDecimal.Parse(cmd.Parameters["P_ID_MODELO"].Value.ToString());

                    await cn.CloseAsync();

                    return (Int64)(OracleDecimal.SetPrecision(orDecimal, 18));
                }
            }
        }
    }
}
