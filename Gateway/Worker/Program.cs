using Binance.Net;
using Binance.Net.Interfaces;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Infrastructure;
using Binebase.Exchange.Common.Infrastructure.Services;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Application.Services;
using Binebase.Exchange.Gateway.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Worker
{
    public static class Program
    {
        public static void Main(string[] args) =>
            CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                    {
                        CommonInfrastructure.ConfigureLogging(hostContext.Configuration, hostContext.HostingEnvironment);

                        services.AddHostedService<Worker>();

                        services.AddSingleton<IBinanceSocketClient, BinanceSocketClient>();
                        services.AddSingleton<ICacheClient, RedisCacheClient>();
                        services.AddSingleton<IExchangeRateProvider, ExchangeRateProvider>();

                        services.AddTransient<IDateTime, DateTimeService>();
                        services.AddTransient<IExchangeRateService, ExchangeRateService>();
                        services.AddTransient<IBinanceClient, BinanceClient>();
                    });
        }
    }
}
