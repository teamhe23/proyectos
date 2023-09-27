using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Data.Interfaces
{
    public interface IDBOracleRepository: IDisposable
    {
        bool EjecutaSPBbook(ref OracleDataReader oracleDataReader, ref OracleCommand OraCommand,ref string ErrorMessage, string SpName, bool conresultado);
        bool EjecutaSPBbook2(ref OracleDataReader oracleDataReader, ref OracleCommand OraCommand,ref string ErrorMessage, string SpName, bool conresultado);
        bool EjecutaSQL(ref OracleDataReader oracleDataReader, ref string ErrorMessage, string SqlQuery);
        bool EjecutaSQL(string SqlQuery, ref string ErrorMessage, ref int FilasAfectadas);
    }
}
