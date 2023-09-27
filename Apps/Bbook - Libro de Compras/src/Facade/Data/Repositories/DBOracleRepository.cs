using System;
using System.Data;
using System.Threading.Tasks;
using IntegracionBbook.Data.Interfaces;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

/// <summary>
/// Clase de Conexión y empleo de la DB Oracle.
/// ODP.NET Oracle managed provider
/// </summary>
namespace IntegracionBbook.Data.Repositories
{
    public class DBOracleRepository : IDBOracleRepository
    {

        // Variables.
        private OracleConnection ora_Connection;
        private OracleTransaction ora_Transaction;
        public OracleDataReader ora_DataReader;
        private readonly IConfiguration _Config;

        private struct stConnDB
        {
            public string CadenaConexion;
            public string ErrorDesc;
            public int ErrorNum;
        }
        private stConnDB info;

        // Indica el numero de intentos de conectar a la BD sin exito.
        public byte ora_intentos = 0;
        // Indica el tiempo de espera de conexion a la BD.
        public int timeout = 300;

        #region "Propiedades"

        /// <summary>
        /// Devuelve la descripcion de error de la clase.
        /// </summary>
        public string ErrDesc
        {
            get { return this.info.ErrorDesc; }
        }

        /// <summary>
        /// Devuelve el numero de error de la clase.
        /// </summary>
        public string ErrNum
        {
            get { return info.ErrorNum.ToString(); }
        }

        #endregion


        /// <summary>
        /// Constructor.
        /// </summary>
        public DBOracleRepository(IConfiguration Config)
        {
            _Config = Config;
            // Creamos la cadena de conexión de la base de datos.
             
            info.CadenaConexion = string.Concat("Data Source=", _Config["DB:DataSource"], "; USER ID=", _Config["DB:UserID"], "; PASSWORD=", _Config["DB:Password"]);

            // Instanciamos objeto conecction.
            ora_Connection = new OracleConnection();
        }

        /// <summary>
        /// Implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose de la clase.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Liberamos objetos manejados.
            }

            try
            {
                // Liberamos los obtetos no manejados.
                if (ora_DataReader != null)
                {
                    ora_DataReader.Close();
                    ora_DataReader.Dispose();
                }

                // Cerramos la conexión a DB.
                if (!Desconectar())
                {
                    // Grabamos Log de Error...
                }

            }
            catch (Exception ex)
            {
                // Asignamos error.
                AsignarError(ref ex);
            }

        }


        /// <summary>
        /// Destructor.
        /// </summary>
        ~DBOracleRepository()
        {
            Dispose(false);
        }


        /// <summary>
        /// Se conecta a una base de datos de Oracle.
        /// </summary>
        /// <returns>True si se conecta bien.</returns>
        private bool Conectar()
        {

            bool ok = false;

            try
            {
                if (ora_Connection != null)
                {
                    // Fijamos la cadena de conexión de la base de datos.
                    ora_Connection.ConnectionString = info.CadenaConexion;
                    ora_Connection.OpenAsync();
                    if(ora_Connection.State == ConnectionState.Open)
                    {
                        ok = true;
                    } 
                }
            }
            catch (Exception ex)
            {
                // Desconectamos y liberamos memoria.
                //Desconectar();
                // Asignamos error.
                AsignarError(ref ex);
                // Asignamos error de función
                ok = false;
            }

            return ok;

        }


        /// <summary>
        /// Cierra la conexión de BBDD.
        /// </summary>
        public bool Desconectar()
        {
            try
            {
                // Cerramos la conexion
                if (ora_Connection != null)
                {
                    if (ora_Connection.State != ConnectionState.Closed)
                    {
                        ora_Connection.Close();
                    }
                }
                // Liberamos su memoria.
                ora_Connection.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                AsignarError(ref ex);
                return false;
            }
        }


        /// <summary>
        /// Ejecuta un procedimiento almacenado de Oracle.
        /// </summary>
        /// <param name="oraCommand">Objeto Command con los datos del procedimiento.</param>
        /// <param name="SpName">Nombre del procedimiento almacenado.</param>
        /// <returns>True si el procedimiento se ejecuto bien.</returns>
        public bool EjecutaSPBbook(ref OracleDataReader oracleDataReader, ref OracleCommand OraCommand, ref string ErrorMessage,string SpName,bool conresultado)
        {
            bool ok = true;

            try
            {
                // Si no esta conectado, se conecta.
                if (!IsConected())
                {
                    ok = Conectar();
                }

                if (ok)
                {
                    ora_Transaction = ora_Connection.BeginTransaction();
                    OraCommand.Connection = ora_Connection;
                    OraCommand.CommandText = "BBOOK_PKG_INTEGRATION." + SpName;
                    OraCommand.CommandType = CommandType.StoredProcedure;
                    OraCommand.CommandTimeout = timeout;
                    if (conresultado)oracleDataReader = OraCommand.ExecuteReader();
                    else OraCommand.ExecuteNonQuery();
                    ora_Transaction.Commit();
                    ErrorMessage = "OK";
                }
                else
                {
                    ok = false;
                    ErrorMessage = "No se puede crear una conexion a Base de datos";
                }
            }
            catch (Exception ex)
            {
                ora_Transaction.Rollback();
                if (ex.Message.Contains("ORA-00001: unique constraint") || ex.Message.Contains("ORA-00001: restricción única")) ErrorMessage = "Clave unica duplicada";
                else ErrorMessage = ex.Message;
                AsignarError(ref ex);
                //ok = false;
            }

            return ok;
        }
        public bool EjecutaSPBbook2(ref OracleDataReader oracleDataReader, ref OracleCommand OraCommand, ref string ErrorMessage, string SpName, bool conresultado)
        {
            bool ok = true;

            try
            {
                // Si no esta conectado, se conecta.
                if (!IsConected())
                {
                    ok = Conectar();
                }

                if (ok)
                {
                    ora_Transaction = ora_Connection.BeginTransaction();
                    OraCommand.Connection = ora_Connection;
                    OraCommand.CommandText = "BBOOK_PKG_INTEGRATION2." + SpName;
                    OraCommand.CommandType = CommandType.StoredProcedure;
                    OraCommand.CommandTimeout = timeout;
                    if (conresultado) oracleDataReader = OraCommand.ExecuteReader();
                    else OraCommand.ExecuteNonQuery();
                    ora_Transaction.Commit();
                    ErrorMessage = "OK";
                }
                else
                {
                    ok = false;
                    ErrorMessage = "No se puede crear una conexion a Base de datos";
                }
            }
            catch (Exception ex)
            {
                ora_Transaction.Rollback();
                if (ex.Message.Contains("ORA-00001: unique constraint") || ex.Message.Contains("ORA-00001: restricción única")) ErrorMessage = "Clave unica duplicada";
                else ErrorMessage = ex.Message;
                AsignarError(ref ex);
                //ok = false;
            }

            return ok;
        }



        /// <summary>
        /// Ejecuta una sql que rellenar un DataReader (sentencia select).
        /// </summary>
        /// <param name="SqlQuery">sentencia sql a ejecutar</param>
        /// <returns></returns> 
        public bool EjecutaSQL(ref OracleDataReader oracleDataReader,ref string ErrorMessage, string SqlQuery)
        {

            bool ok = true;

            OracleCommand ora_Command = new OracleCommand();

            try
            {

                // Si no esta conectado, se conecta.
                if (!IsConected())
                {
                    ok = Conectar();
                }

                if (ok)
                {
                    // Cerramos cursores abiertos, para evitar el error ORA-1000
                    if ((ora_DataReader != null))
                    {
                        ora_DataReader.Close();
                        ora_DataReader.Dispose();
                    }

                    ora_Transaction = ora_Connection.BeginTransaction();
                    ora_Command.Connection = ora_Connection;
                    ora_Command.CommandType = CommandType.Text;
                    ora_Command.CommandText = SqlQuery;
                    ora_Command.CommandTimeout = timeout;

                    // Ejecutamos sql.
                    ora_DataReader = ora_Command.ExecuteReader();
                    oracleDataReader = ora_DataReader;
                    ora_Transaction.Commit();
                    ErrorMessage = "OK";
                }
                else
                {
                    ErrorMessage = "No se puede crear una conexion a Base de datos";
                    ok = false;
                }
            }
            catch (Exception ex)
            {
                AsignarError(ref ex);
                ora_Transaction.Rollback();
                //ok = false;
                if (ex.Message.Contains("ORA-00001: unique constraint") || ex.Message.Contains("ORA-00001: restricción única")) ErrorMessage = "Clave unica duplicada";
                else ErrorMessage = ex.Message;
            }
            finally
            {
                if (ora_Command != null)
                {
                    ora_Command.Dispose();
                }
            }

            return ok;

        }



        /// <summary>
        /// Ejecuta una sql que no devuelve datos (update, delete, insert).
        /// </summary>
        /// <param name="SqlQuery">sentencia sql a ejecutar</param>
        /// <param name="ErrorMessage">Error que devuelve el Oracle</param>
        /// <param name="FilasAfectadas">Fila afectadas por la sentencia SQL</param>
        /// <returns></returns>
        public bool EjecutaSQL(string SqlQuery, ref string ErrorMessage, ref int FilasAfectadas)
        {

            bool ok = true;
            OracleCommand ora_Command = new OracleCommand();

            try
            {

                // Si no esta conectado, se conecta.
                if (!IsConected())
                {
                    ok = Conectar();
                }

                if (ok)
                {
                    ora_Transaction = ora_Connection.BeginTransaction();
                    ora_Command = ora_Connection.CreateCommand();
                    ora_Command.CommandType = CommandType.Text;
                    ora_Command.CommandText = SqlQuery;
                    ora_Command.CommandTimeout = timeout;
                    FilasAfectadas = ora_Command.ExecuteNonQuery();
                    ora_Transaction.Commit();
                    ErrorMessage = "OK";
                }
                else
                {
                    ErrorMessage = "No se puede crear una conexion a Base de datos";
                    ok = false;
                }
            }
            catch (Exception ex)
            {
                // Hacemos rollback.
                ora_Transaction.Rollback();
                AsignarError(ref ex);
                //ok = false;
                if (ex.Message.Contains("ORA-00001: unique constraint") || ex.Message.Contains("ORA-00001: restricción única")) ErrorMessage = "Clave unica duplicada";
                else ErrorMessage = ex.Message;
            }
            finally
            {
                // Recolectamos objetos para liberar su memoria.
                if (ora_Command != null)
                {
                    ora_Command.Dispose();
                }
            }

            return ok;

        }


        /// <summary>
        /// Captura Excepciones
        /// </summary>
        /// <param name="ex">Excepcion producida.</param>
        private void AsignarError(ref Exception ex)
        {
            // Si es una excepcion de Oracle.
            if (ex is OracleException)
            {
                info.ErrorNum = ((OracleException)ex).Number;
                info.ErrorDesc = ex.Message;
            }
            else
            {
                info.ErrorNum = 0;
                info.ErrorDesc = ex.Message;
            }
            // Grabamos Log de Error...
        }



        /// <summary>
        /// Devuelve el estado de la base de datos
        /// </summary>
        /// <returns>True si esta conectada.</returns>
        public bool IsConected()
        {

            bool ok = false;

            try
            {
                // Si el objeto conexion ha sido instanciado
                if (ora_Connection != null)
                {
                    // Segun el estado de la Base de Datos.
                    switch (ora_Connection.State)
                    {
                        case ConnectionState.Closed:
                        case ConnectionState.Broken:
                        case ConnectionState.Connecting:
                            ok = false;
                            break;
                        case ConnectionState.Open:
                        case ConnectionState.Fetching:
                        case ConnectionState.Executing:
                            ok = true;
                            break;
                    }
                }
                else
                {
                    ok = false;
                }

            }
            catch (Exception ex)
            {
                AsignarError(ref ex);
                ok = false;
            }

            return ok;

        }

    }

}
