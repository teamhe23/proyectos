
namespace Domain.Models.Properties
{
    public class OracleProperty
    {
        public string DataSource        { get; set; } = string.Empty;
        public string User              { get; set; } = string.Empty;
        public string Password          { get; set; } = string.Empty;
        public string stringConexion    { get; set; } = string.Empty;

        public void SetConnection()
        {
            stringConexion = $"data source={DataSource};user id={User};password={Password};";
        }
    }
}
