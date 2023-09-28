using Domain.Services;
using IoC;
using Microsoft.Extensions.DependencyInjection;

namespace Facade
{
    public class Program
    {
        public static async Task Main()
        {
            try
            {
                var Services = IoCBootstrapper.Bootstrap()
                                .BuildServiceProvider();

                var PrintService = Services.GetService<IPrinterService>();
                var PubSubService = Services.GetService<IPubSubService>();

                PrintService.PrintInfoJson();
                await PubSubService.ExtraerTrama();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }
    }
}