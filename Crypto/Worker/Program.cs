using Binebase.Exchange.Common.Application;
using Binebase.Exchange.CryptoService.Infrastructure;
using Binebase.Exchange.CryptoService.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Binebase.Exchange.Crypto.Worker
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddInfrastructure(hostContext.Configuration);
                    services.AddConfigurationProviders(hostContext.Configuration);
                    services.Configure<AccountService.Configuration>(hostContext.Configuration.GetSection("AccountService.Configuration"));
                });
    }
}
