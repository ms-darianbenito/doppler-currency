using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Doppler.Currency
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((hostContext, loggerConfiguration) => 
                    loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration))
                .ConfigureAppConfiguration(configHost =>
                {
                    configHost.AddJsonFile("/run/secrets/appsettings.Secret.json", true);
                    configHost.AddKeyPerFile("/run/secrets", true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging((hostingContext, builder) => {});
    }
}
