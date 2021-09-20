using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using  TestService.AuthenticationHandler;
using Serilog;
using Serilog.Events;
using System.IO;

namespace TestService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (!Directory.Exists("logs"))
            {
                Directory.CreateDirectory("logs");
            }

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel
            .Debug()
            .MinimumLevel
            .Override("Microsoft", LogEventLevel.Information)
            .Enrich
            .FromLogContext()
            .WriteTo
            .File(path: $"logs//{DateTimeOffset.Now.ToString("yyyy-MM-dd_hh-mm-ss")}-logs.txt", shared: true)
            .WriteTo.Console()
            .CreateLogger();

            try
            {
                Log.Information("Starting up the service");
                CreateHostBuilder(args).Build().Run();
                return;
            }
            catch (Exception e)
            {
                Log.Fatal(e, "There was a problem starting the service.");
                throw;
            }finally {
                Log.CloseAndFlush();
            }
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    var apiAuthenticationOptions = new ApiAuthenticationOptions();
                    hostContext.Configuration.GetSection("ApiAuthentication").Bind(apiAuthenticationOptions);

                    services.AddHttpClient<ApiServiceClient>(client => client.BaseAddress = new Uri(apiAuthenticationOptions.UrlToService))
                    .AddAuthentication(new ApiCredentials() {
                        Grant=apiAuthenticationOptions.Grant,
                        Password = apiAuthenticationOptions.Password,
                        Username = apiAuthenticationOptions.Username,
                    }, apiAuthenticationOptions.UrlToAuthenticationProvider);

                    services.AddHostedService<ExampleBackgroundWorker>();
                });
    }
}
