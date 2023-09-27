using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Data.Oracle.Helpers
{
    public static class OracleConvert
    {
        public static List<TModel> CursorToModel<TModel>(OracleParameter cursor) where TModel : class
        {
            var reader = ((OracleRefCursor)cursor.Value).GetDataReader();
            var data = new DataTable();
            data.Load(reader);

            return JsonConvert.DeserializeObject<List<TModel>>(JsonConvert.SerializeObject(data, Formatting.Indented));
        }
    }
}
