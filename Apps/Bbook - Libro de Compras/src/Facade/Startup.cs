using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IntegracionBbook.Api.Repositories.Interfaces;
using IntegracionBbook.Api.Repositories.Repositories;
using IntegracionBbook.Data.Interfaces;
using IntegracionBbook.Data.Repositories;
using IntegracionBbook.Repositories.Interfaces;
using IntegracionBbook.Repositories.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace IntegracionBbook
{
    public class Startup
    {
        public Startup(IConfiguration cnfs, IHostingEnvironment env)
        {

            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) 
            .AddEnvironmentVariables();
            Configuration = builder.AddInMemoryCollection(cnfs.AsEnumerable()).Build();

            JsonConvert.DefaultSettings = () => {
                var settings = new JsonSerializerSettings()
                {
                    DateFormatString = Configuration["DateTimeFormat"]
                }; return settings;
            };
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddScoped<IStoreRepository,StoreRepository>();
            services.AddScoped<IBrandRepository,BrandRepository>();
            services.AddScoped<IBuyerRepository,BuyerRepository>();
            services.AddScoped<IVendorRepository,VendorRepository>();
            services.AddScoped<IHierarchyRepository,HierarchyRepository>();
            services.AddScoped<IDimensionRepository,DimensionRepository>();
            services.AddScoped<ISizeRepository,SizeRepository>();
            services.AddScoped<IComexRepository,ComexRepository>();
            services.AddScoped<ISeasonRepository,SeasonRepository>();
            services.AddScoped<IMaster_poRepository,Master_poRepository>();
            services.AddScoped<IIn_poRepository,In_PoRepository>();
            services.AddScoped<IIn_AsnRepository, In_AsnRepository>();
            services.AddScoped<IIn_ComexRepository, In_ComexRepository>();
            services.AddScoped<IIn_CodesRepository,In_CodesRepository>();
            services.AddScoped<IReceived_productRepository,Received_productRepository>();
            services.AddScoped<IProductRepository,ProductRepository>();
            services.AddScoped<IProduct_modificationRepository,Product_modificationRepository>();
            services.AddScoped<IDBOracleRepository, DBOracleRepository>();
            services.AddScoped<ICommonRepository, CommonRepository>();
            services.AddCors(opciones =>
            {
                opciones.AddPolicy("AllowMyOrigin",
                constructor => constructor.AllowAnyOrigin().AllowAnyHeader());
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyTestService", Version = "v1", });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseDeveloperExceptionPage();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TestService");
            });
        }
    }
}
