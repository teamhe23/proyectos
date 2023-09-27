using Data.Oracle.Helpers;
using Domain.Models;
using Domain.Models.Properties;
using Domain.Repositories.Oracle;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Oracle.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OracleProperties _oracleProperties;
        public OrderRepository(IOptions<OracleProperties> oracleProperties)
        {
            _oracleProperties = oracleProperties.Value;
            _oracleProperties.SetConnection();
        }

        public async Task<List<Transferencia>> GetTransferencia()
        {
            using var cn = new OracleConnection(_oracleProperties.stringConexion);

            using var cmd = cn.CreateCommand();

            await cn.OpenAsync();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TIENDAS_ADM.WMS_INTEGRACION_API.SP_SEL_ORDER_RETORNO";
            cmd.CommandTimeout = 60000;

            cmd.Parameters.Add("ORDENES", OracleDbType.RefCursor, ParameterDirection.Output);

            await cmd.ExecuteNonQueryAsync();

            var response = OracleConvert.CursorToModel<Transferencia>(cmd.Parameters["ORDENES"]);

            await cn.CloseAsync();

            return response;
        }

        public async Task<Model> GetModel(int P_ID_RET, string P_IND_ORD)
        {
            using var cn = new OracleConnection(_oracleProperties.stringConexion);

            using var cmd = cn.CreateCommand();

            await cn.OpenAsync();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TIENDAS_ADM.WMS_INTEGRACION_API.SP_GET_ORDER_RETORNO";
            cmd.CommandTimeout = 60000;

            cmd.Parameters.Add("P_ID_RET", OracleDbType.Int32).Value = P_ID_RET;
            cmd.Parameters.Add("P_IND_ORD", OracleDbType.Varchar2).Value = P_IND_ORD;

            cmd.Parameters.Add("CABECERA", OracleDbType.RefCursor, ParameterDirection.Output);
            cmd.Parameters.Add("DETALLE", OracleDbType.RefCursor, ParameterDirection.Output);

            await cmd.ExecuteNonQueryAsync();

            var Valor1 = string.Empty;
            var Valor2 = string.Empty;
            var valorBIR = string.Empty;

            foreach (var item in OracleConvert.CursorToModel<Model1>(cmd.Parameters["CABECERA"]))
            {
                Valor1 = item.REGISTRO;
                Int32 posBIR = Valor1.IndexOf("BIR");
                valorBIR = Valor1.Substring(posBIR, 18);
            }

            foreach (var item in OracleConvert.CursorToModel<Model2>(cmd.Parameters["DETALLE"]))
            {
                Valor2 += $"{item.REGISTRO}{Environment.NewLine}";
            }

            await cn.CloseAsync();
            return new Model
            {
                data = $"{Valor1}{Environment.NewLine}{Valor2}",
                nroBIR = valorBIR
            };
        }

        public async Task<bool> Confirma_Envio(int P_ID_RET, string P_IND_ORD, String nroBIR)
        {
            using var cn = new OracleConnection(_oracleProperties.stringConexion);

            using var cmd = cn.CreateCommand();

            await cn.OpenAsync();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TIENDAS_ADM.WMS_INTEGRACION_API.SP_UPD_ORDER_RETORNO_ENVIO";
            cmd.CommandTimeout = 60000;

            cmd.Parameters.Add("P_ID_RET", OracleDbType.Int32).Value = P_ID_RET;
            cmd.Parameters.Add("P_IND_ORD", OracleDbType.Varchar2).Value = P_IND_ORD;
            cmd.Parameters.Add("P_BIR_NBR", OracleDbType.Varchar2).Value = nroBIR;

            int result = await cmd.ExecuteNonQueryAsync();

            await cn.CloseAsync();

            return true;
        }
    }
}
