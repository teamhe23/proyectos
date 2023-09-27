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
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Data.Oracle.Repositories
{
    public class IncotermRepository:IIncotermRepository
    {
        private readonly OracleProperty oracleProperty;
        private readonly SettingProperty settingProperty;

        public IncotermRepository(
            IOptions<OracleProperty> oracleProperty,
            IOptions<SettingProperty> settingProperty
            )
        {
            this.oracleProperty = oracleProperty.Value;
            this.oracleProperty.CrearCadenaConexion();
            this.settingProperty = settingProperty.Value;
        }

        public async Task<List<Incoterm>> ListaIncoterm()
        {
            using (var cn = new OracleConnection(oracleProperty.CadenaConexion))
            {
                using (var cmd = cn.CreateCommand())
                {
                    await cn.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "edsr.PKG_INTEGRACIONES_LOGISTICAS.SP_SEL_INCOTERM";
                    cmd.CommandTimeout = settingProperty.TiempoEsperaBdSegundos;

                    cmd.Parameters.Add("P_LSTINCOTERM", OracleDbType.RefCursor, ParameterDirection.Output);

                    await cmd.ExecuteNonQueryAsync();

                    var Incoterm = OracleConvert.CursorToModel<Incoterm>(cmd.Parameters["P_LSTINCOTERM"]);
                    await cn.CloseAsync();

                    return Incoterm;
                }
            }
        }


    }
}
