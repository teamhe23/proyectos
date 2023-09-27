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
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Data.Oracle.Repositories
{
    public class PlazosRepository :IPlazosRepository
    {
        private readonly OracleProperty oracleProperty;
        private readonly SettingProperty settingProperty;

        public PlazosRepository(
            IOptions<OracleProperty> oracleProperty,
            IOptions<SettingProperty> settingProperty
        )
        {
            this.oracleProperty = oracleProperty.Value;
            this.oracleProperty.CrearCadenaConexion();
            this.settingProperty = settingProperty.Value;
        }
        public async Task<List<Plazos>> ListarPlazos(string pFiltro)
        {
            using (var cn = new OracleConnection(oracleProperty.CadenaConexion))
            {
                List<Plazos> Plazos = new List<Plazos>();

                using (var cmd = cn.CreateCommand())
                {
                    await cn.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "EDSR.PKG_INTEGRACIONES_LOGISTICAS.SP_SEL_PLAZOS";
                    cmd.CommandTimeout = settingProperty.TiempoEsperaBdSegundos;

                    cmd.Parameters.Add(new OracleParameter("pFiltro", OracleDbType.Varchar2, ParameterDirection.Input) { Value = pFiltro });
                    cmd.Parameters.Add("PLAZOS", OracleDbType.RefCursor, ParameterDirection.Output);
                    await cmd.ExecuteNonQueryAsync();

                    OracleDataReader dataReader = ((OracleRefCursor)cmd.Parameters["PLAZOS"].Value).GetDataReader();

                    if (dataReader != null && dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            Plazos.Add(new Plazos()
                            {
                                codigo = Int32.Parse(dataReader["CODIGO"].ToString()),
                                descripcion = dataReader["DESCRIPCION"].ToString()
                            });
                        }
                    }
                    await cn.CloseAsync();
                    return Plazos;
                }
            }
        }
    }
}
