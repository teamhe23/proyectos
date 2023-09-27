
namespace Domain.Models
{
    public class ModeloRequest
    {
        public Int64    IdModelo    { get; set; }
        public int      IdTipo      { get; set; }
        public string   Modelo      { get; set; } = string.Empty;
        public string   MessageId   { get; set; } = string.Empty;
    }
}
