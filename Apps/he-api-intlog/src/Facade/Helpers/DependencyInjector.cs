using Data.Oracle.Repositories;
using Domain.Models;
using Domain.Repositories;
using Domain.Services;
using Service.Services;

namespace Facade.Helpers
{
    public static class DependencyInjector
    {
        public static IServiceCollection AddServices(this IServiceCollection service)
        {
            service.AddTransient<IMetodoDistribucionService, MetodoDistribucionService>();
            service.AddTransient<IProveedorService, ProveedorService>();
            service.AddTransient<ITipoOrdenCompraService, TipoOrdenCompraService>();
            service.AddTransient<ITiendaService, TiendaService>();
            service.AddTransient<ICompradorService, CompradorService>();
            service.AddTransient<IPlazosService, PlazosService>();
            service.AddTransient<IProductoService, ProductoService>();
            service.AddTransient<IOrdenCompraService, OrdenCompraService>();
            service.AddTransient<IIncotermService, IncotermService>();
            service.AddTransient<IPuertoService, PuertoService>();
            service.AddTransient<ICadenaPrecioService, CadenaPrecioService>();
            service.AddTransient<IFechaPMMService, FechaPMMService>();
            service.AddTransient<ISucursalesService, SucursalesService>();
            service.AddTransient<IPrioridadDePrecioService, PrioridadDePrecioService>();
            service.AddTransient<IPrecioService, PrecioService>();
            service.AddTransient<IPrecioEnLineaService, PrecioEnLineaService>();

            return service;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection service)
        {
            service.AddSingleton<IMetodoDistribucionRepository, MetodoDistribucionRepository>();
            service.AddSingleton<IProveedorRepository, ProveedorRepository>();
            service.AddSingleton<ITipoOrdenCompraRepository, TipoOrdenCompraRepository>();
            service.AddSingleton<ITiendaRepository, TiendaRepository>();
            service.AddSingleton<ICompradorRepository, CompradorRepository>();
            service.AddSingleton<IPlazosRepository, PlazosRepository>();
            service.AddSingleton<IProductoRepository, ProductoRepository>();
            service.AddSingleton<IOrdenCompraRepository, OrdenCompraRepository>();
            service.AddSingleton<IIncotermRepository, IncotermRepository>();
            service.AddSingleton<IPuertoRepository, PuertoRepository>();
            service.AddSingleton<ICadenaPrecioRepository, CadenaPrecioRepository>();
            service.AddSingleton<IFechaPMMRepository, FechaPMMRepository>();
            service.AddSingleton<ISucursalesRepository, SucursalesRepository>();
            service.AddSingleton<IPrioridadDePrecioRepository, PrioridadDePrecioRepository>();
            service.AddSingleton<IPrecioRepository, PrecioRepository>();
            service.AddSingleton<IPrecioEnLineaRepository, PrecioEnLineaRepository>();

            return service;
        }
    }
}
