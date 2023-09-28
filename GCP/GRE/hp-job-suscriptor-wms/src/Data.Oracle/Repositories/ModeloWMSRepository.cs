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
    public class ModeloWMSRepository : IModeloWMSRepository
    {
        private readonly OracleProperties _oracleProperties;
        private readonly Settigns _settigns;

        public ModeloWMSRepository(IOptions<OracleProperties> oracleProperties,
                                   IOptions<Settigns> settigns)
        {
            _oracleProperties = oracleProperties.Value;
            _oracleProperties.SetConnection();
            _settigns = settigns.Value;
        }

        public async Task<Int64> InsModeloWMS(ModeloWNS model)
        {
            using (var cn = new OracleConnection(_oracleProperties.stringConexion))
            {
                using (var cmd = new OracleCommand())
                {
                    await cn.OpenAsync();

                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "EDSR.PKG_GRE.SP_INS_MODELO_WMS";
                    cmd.CommandTimeout = _settigns.TiempoEsperaSegundos;

                    cmd.Parameters.Add("P_VC_CADENA", OracleDbType.Clob).Value = model.Cadena;
                    cmd.Parameters.Add("P_NU_ID_MODELO", OracleDbType.Int64, ParameterDirection.Output);
                    await cmd.ExecuteNonQueryAsync();

                    var orDecimal = OracleDecimal.Parse(cmd.Parameters["P_NU_ID_MODELO"].Value.ToString());

                    if (cn.State == ConnectionState.Open)
                    {
                        cn.Close();
                    }

                    return (Int64)(OracleDecimal.SetPrecision(orDecimal, 18));
                }
            }
        }
    }
}
