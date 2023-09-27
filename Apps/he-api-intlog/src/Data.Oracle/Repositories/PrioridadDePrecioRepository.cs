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
    public class PrioridadDePrecioRepository : IPrioridadDePrecioRepository
    {
        private readonly OracleProperty oracleProperty;
        private readonly SettingProperty settingProperty;

        public PrioridadDePrecioRepository(
            IOptions<OracleProperty> oracleProperty,
            IOptions<SettingProperty> settingProperty
            )
        {
            this.oracleProperty = oracleProperty.Value;
            this.oracleProperty.CrearCadenaConexion();
            this.settingProperty = settingProperty.Value;
        }

        public async Task<List<PrioridadDePrecio>> ListarTipoPrecio()
        {
            using (var cn = new OracleConnection(oracleProperty.CadenaConexion))
            {
                using (var cmd = cn.CreateCommand())
                {
                    await cn.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "edsr.PKG_INTEGRACIONES_LOGISTICAS.SP_GET_TIPO_PRECIO";
                    cmd.CommandTimeout = settingProperty.TiempoEsperaBdSegundos;

                    cmd.Parameters.Add("TIPOPRECIO", OracleDbType.RefCursor, ParameterDirection.Output);

                    await cmd.ExecuteNonQueryAsync();

                    var tipoprecio = OracleConvert.CursorToModel<PrioridadDePrecio>(cmd.Parameters["TIPOPRECIO"]);
                    await cn.CloseAsync();

                    return tipoprecio;
                }
            }
        }
    }
}

   

