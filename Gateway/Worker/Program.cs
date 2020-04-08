using AutoMapper;
using Binance.Net;
using Binance.Net.Interfaces;
using Binebase.Exchange.Common.Application;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Infrastructure;
using Binebase.Exchange.Common.Infrastructure.Services;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Application.Services;
using Binebase.Exchange.Gateway.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Binebase.Exchange.Gateway.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Binebase.Exchange.Gateway.Worker;
using Binebase.Exchange.Gateway.Infrastructure.Interfaces;

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
                        services.AddCommonInfrastructure();
                        services.AddHostedService<Worker>();

                        services.AddSingleton<ICacheClient, RedisCacheClient>();
                        services.AddTransient<IExchangeRateService, ExchangeRateService>();
                        services.AddTransient<IExchangeRateProvider, ExchangeRateProvider>();

                        services.AddSingleton<IBinanceSocketClient, BinanceSocketClient>();
                        services.AddTransient<IBinanceClient, BinanceClient>();

                        services.AddTransient<ITransactionsSyncService, TransactionsSyncService>();
                        services.AddHttpClient<ICryptoService, CryptoService>().AddPolicyHandler(CommonInfrastructure.GetRetryPolicy());

                        services.AddDbContext<ApplicationDbContext>(x => x.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection"),
                             b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

                        services.AddTransient<ICurrentUserService, SystemUserService>();
                        services.AddTransient<IApplicationDbContext, ApplicationDbContext>();
                        services.AddTransient<IUserContext, ApplicationDbContext>();

                        services.AddConfigurationProviders(hostContext.Configuration);
                        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
                        services.Configure<CryptoService.Configuration>(hostContext.Configuration.GetSection("CryptoService.Configuration"));
                    });
        }
    }
}
