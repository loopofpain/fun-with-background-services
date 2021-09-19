using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using  TheWindowsService.Test2.AuthenticationHandler;

namespace TheWindowsService.Test2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
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

                    services.Configure<ExampleWorkerOptions>(hostContext.Configuration.GetSection("ExampleWorkerOptions"));
                    services.AddHostedService<ExampleWorker>();
                });
    }
}
