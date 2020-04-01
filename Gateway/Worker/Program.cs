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
using Serilog;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;

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
                        var env = hostContext.HostingEnvironment;
                        var configuration = hostContext.Configuration;
                        ConfigureLogging(configuration, env);

                        services.AddHostedService<Worker>();

                        services.AddTransient<IDateTime, DateTimeService>();
                        services.AddSingleton<ICacheClient, RedisCacheClient>();
                        services.AddTransient<IExchangeRateService, ExchangeRateService>();
                        services.AddTransient<IExchangeRateProvider, ExchangeRateProvider>();

                        services.AddSingleton<IBinanceSocketClient, BinanceSocketClient>();
                        services.AddTransient<IBinanceClient, BinanceClient>();

                        services.AddConfigurationProviders(hostContext.Configuration);
                    });
        }

        private static void ConfigureLogging(IConfiguration configuration, IHostEnvironment environment)
        {
            LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
                 .Enrich.FromLogContext()
                 .Enrich.WithExceptionDetails()
                 .Enrich.WithMachineName()
                 .WriteTo.Debug()
                 .WriteTo.Console()
                 .WriteTo.File(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $"\\logs\\{DateTime.UtcNow:yyyyMMddHHmm}log.log")
                 .Enrich.WithProperty("Environment", environment)
                 .ReadFrom.Configuration(configuration);

            if (environment.IsProduction())
            {
                loggerConfiguration.WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment.EnvironmentName));
            }

            Log.Logger = loggerConfiguration.CreateLogger();
        }

        private static ElasticsearchSinkOptions ConfigureElasticSink(IConfiguration configuration, string environment)
        {
            return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
            };
        }
    }
}
