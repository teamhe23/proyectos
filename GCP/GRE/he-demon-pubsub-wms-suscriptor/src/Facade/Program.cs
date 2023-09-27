using Domain.Services;
using Facade.Helpers;
using Microsoft.Extensions.DependencyInjection;

try
{
    var Services = IoCBootstrapper.Bootstrap()
                    .BuildServiceProvider();

    var PrintService = Services.GetService<IPrinterService>();
    var pubSubService = Services.GetService<IPubSubService>();

    PrintService.PrintInfoJson();
    PrintService.Print("Inicio proceso v1.0.0");
    await pubSubService.ExtraerTrama();
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex.Message);
}
