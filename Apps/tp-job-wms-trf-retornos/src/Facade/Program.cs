using Domain.Models;
using Domain.Services;
using IoC;
using Jobs.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Facade
{
    class Program
    {
        public static async Task Main()
        {
            try
            {
                var Services = IoCBootstrapper.Bootstrap()
                                .BuildServiceProvider();

                var PrintService = Services.GetService<IPrinterService>();
                var SettignsService = Services.GetService<ISettignsService>();
                var settign = SettignsService.GetSettings();
                var WMSService = Services.GetService<IWMSService>();
                PrintService.PrintInfoJson();

                do
                {
                    var LsIntegracion = await WMSService.getIntegracion();

                    int initial = 0;
                    int end = 0;

                    foreach (TipoIntegracion obj in LsIntegracion)
                    {
                        initial = Int32.Parse(obj.HORA_INICIO);
                        end = Int32.Parse(obj.HORA_FINAL);
                    }

                    if (DateTime.Now.Hour >= initial && DateTime.Now.Hour <= end)
                    {
                        PrintService.PrintInicioWMS();

                        Model Model = new Model();
                        String nroBIR = String.Empty;

                        foreach (var transferencia in await WMSService.GetTransferencia())
                        {
                            Model = await WMSService.GetModel(transferencia.ID_RET, transferencia.IND_ORD);



                            if (Model.data.Length > 0)
                            {
                                await WMSService.PostAllOrder(Model, transferencia.ID_RET, transferencia.IND_ORD);
                            }
                        }

                        PrintService.PrintFinWMS();

                        Thread.Sleep(settign.TiempoEsperaSegundos * 1000);
                    }
                } while (true);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }
    }
}
