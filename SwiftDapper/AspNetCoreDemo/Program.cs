using AspNetCoreDemo.Database;
using AspNetCoreDemo.Repositories.Contracts;
using AspNetCoreDemo.Repositories;
using AspNetCoreDemo.Services.Contracts;
using AspNetCoreDemo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using AspNetCoreDemo.Database.Contracts;
using NLog;
using System.IO;

namespace AspNetCoreDemo
{
	public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "AspNetCoreDemo API", Version = "v1" });
            });

            builder.Services.AddSingleton(new DatabaseConfig { Name = builder.Configuration["DatabaseName"] });
            builder.Services.AddScoped<IDatabaseBootstrap, DatabaseBootstrap>();
            builder.Services.AddScoped<ISwiftRepository, SwiftRepository>();
            builder.Services.AddScoped<ISwiftService, SwiftService>();
            builder.Services.AddSingleton<ILoggerManager, LoggerManager>();
            
            var app = builder.Build();

            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "AspNetCoreDemo API V1");
                options.RoutePrefix = "api/swagger";
            });

            app.UseStaticFiles();

            var serviceProvider = app.Services;
            serviceProvider.GetService<IDatabaseBootstrap>().Setup();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

            app.Run();
        }
    }
}
