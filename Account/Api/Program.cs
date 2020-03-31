using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Binebase.Exchange.AccountService.Api
{
    public sealed class Program
    {
        public static void Main(string[] args) 
            => CreateWebHostBuilder(args).Build().Run();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
             .ConfigureLogging(logging =>
             {
                 logging.ClearProviders();
                 logging.AddConsole();
                 logging.AddAzureWebAppDiagnostics();
                 logging.AddEventSourceLogger();
             })
                .UseStartup<Startup>();
    }
}
