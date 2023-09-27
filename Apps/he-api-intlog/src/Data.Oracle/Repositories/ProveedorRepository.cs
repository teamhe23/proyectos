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
    public class ProveedorRepository : IProveedorRepository
    {
        private readonly OracleProperty oracleProperty;
        private readonly SettingProperty settingProperty;

        public ProveedorRepository(
            IOptions<OracleProperty> oracleProperty,
            IOptions<SettingProperty> settingProperty
        )
        {
            this.oracleProperty = oracleProperty.Value;
            this.oracleProperty.CrearCadenaConexion();
            this.settingProperty = settingProperty.Value;
        }

        public async Task<List<Proveedor>> BuscarProveedor(string pFiltro)
        {
            using (var cn = new OracleConnection(oracleProperty.CadenaConexion))
            {
                List<Proveedor> Proveedor = new List<Proveedor>();

                using (var cmd = cn.CreateCommand())
                {
                    await cn.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "EDSR.PKG_INTEGRACIONES_LOGISTICAS.SP_SEL_PROVEEDORES_BUSCAR";
                    cmd.CommandTimeout = settingProperty.TiempoEsperaBdSegundos;

                    cmd.Parameters.Add(new OracleParameter("PFILTRO", OracleDbType.Varchar2, ParameterDirection.Input) { Value = pFiltro });
                    cmd.Parameters.Add("PROVEEDORES", OracleDbType.RefCursor, ParameterDirection.Output);
                    await cmd.ExecuteNonQueryAsync();

                    OracleDataReader dataReader = ((OracleRefCursor)cmd.Parameters["PROVEEDORES"].Value).GetDataReader();

                    if (dataReader != null && dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            Proveedor.Add(new Proveedor()
                            {
                                vpc_tech_key = Int32.Parse(dataReader["VPC_TECH_KEY"].ToString()),
                                codigo = dataReader["CODIGO"].ToString(),
                                descripcion = dataReader["DESCRIPCION"].ToString(),
                                pais = Int32.Parse(dataReader["PAIS"].ToString()),
                                procedencia = dataReader["PROCEDENCIA"].ToString(),
                                dias_cancelacion = Int32.Parse(dataReader["DIAS_CANCELACION"].ToString()),
                                plazo = dataReader["PLAZO"].ToString(),
                                vpc_status_key = Int32.Parse(dataReader["VPC_STATUS_KEY"].ToString()),
                                moneda= dataReader["CURR_CODE"].ToString()
                            });
                        }
                    }
                    await cn.CloseAsync();
                    return Proveedor;
                }
            }
        }      
    }
}
