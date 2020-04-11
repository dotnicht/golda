using Binebase.Exchange.Common.Infrastructure;
using Binebase.Exchange.CryptoService.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Binebase.Exchange.Crypto.Worker
{
    public sealed class Program
    {
        public static void Main(string[] args)
            => CreateHostBuilder(args).Build().Run();


        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddInfrastructure(hostContext.Configuration);
                    CommonInfrastructure.ConfigureLogging(hostContext.Configuration, hostContext.HostingEnvironment);
                });
    }
}
