using Binance.Net;
using Binance.Net.Interfaces;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Application.Services;
using Binebase.Exchange.Common.Application;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Gateway.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Binebase.Exchange.Common.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace Worker
{
    public static class Program
    {
        public static void Main(string[] args)
            => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                    {
                        services.AddHostedService<Worker>();

                        services.AddTransient<IDateTime, DateTimeService>();
                        services.AddSingleton<ICacheClient, RedisCacheClient>();
                        services.AddTransient<IExchangeRateService, ExchangeRateService>();
                        services.AddTransient<IExchangeRateProvider, ExchangeRateProvider>();

                        services.AddSingleton<IBinanceSocketClient, BinanceSocketClient>();
                        services.AddTransient<IBinanceClient, BinanceClient>();

                        services.AddConfigurationProviders(hostContext.Configuration);
                    })
             .ConfigureLogging((hostngContext, logging) =>
                     {
                         logging.AddConfiguration(hostngContext.Configuration.GetSection("Logging"));
                         logging.ClearProviders();
                         logging.AddConsole();
                         logging.AddAzureWebAppDiagnostics();
                         logging.AddEventSourceLogger();
                     });
    }
}
