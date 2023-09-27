
namespace Domain.Services
{
    public interface IPrinterService
    {
        void Print(string message);
        void PrintInfoJson();
        void PrintInicioWMS();
        void PrintFinWMS();
    }
}
