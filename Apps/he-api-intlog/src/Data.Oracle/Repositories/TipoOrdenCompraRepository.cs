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
    public class TipoOrdenCompraRepository: ITipoOrdenCompraRepository
    {
        private readonly OracleProperty oracleProperty;
        private readonly SettingProperty settingProperty;

        public TipoOrdenCompraRepository(
            IOptions<OracleProperty> oracleProperty,
            IOptions<SettingProperty> settingProperty
            )
        {
            this.oracleProperty = oracleProperty.Value;
            this.oracleProperty.CrearCadenaConexion();
            this.settingProperty = settingProperty.Value;
        }

        public async Task<List<TipoOrdenCompra>>ListarTipoOrdenCompra(string pOrigen)
        {
            using (var cn = new OracleConnection(oracleProperty.CadenaConexion))
            {
                List<TipoOrdenCompra> TipoOrdenCompra = new List<TipoOrdenCompra>();

                using (var cmd = cn.CreateCommand())
                {
                    await cn.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "EDSR.PKG_INTEGRACIONES_LOGISTICAS.SP_SEL_TIPO_ORDENCOMRPA";
                    cmd.CommandTimeout = settingProperty.TiempoEsperaBdSegundos;
                    cmd.Parameters.Add(new OracleParameter("PORIGEN", OracleDbType.Varchar2, ParameterDirection.Input) { Value = pOrigen });
                    cmd.Parameters.Add("TIPOOC", OracleDbType.RefCursor, ParameterDirection.Output);

                    await cmd.ExecuteNonQueryAsync();

                    OracleDataReader dataReader = ((OracleRefCursor)cmd.Parameters["TIPOOC"].Value).GetDataReader();

                    if (dataReader != null && dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            TipoOrdenCompra.Add(new TipoOrdenCompra()
                            {
                                codigo = Int32.Parse(dataReader["CODIGO"].ToString()),
                                descripcion = dataReader["DESCRIPCION"].ToString(),                                
                            });
                        }
                    }
                    await cn.CloseAsync();
                    return TipoOrdenCompra;
                }
            }
        }

    }
}
