
namespace Domain.Models.Properties
{
    public class OracleProperties
    {
        public string DataSource        { get; set; }
        public string User              { get; set; }
        public string Password          { get; set; }
        public string stringConexion    { get; set; }

        public void SetConnection()
        {
            stringConexion = $"data source={DataSource};user id={User};password={Password};";
        }
    }
}
