using AutoMapper;
using Binance.Net;
using Binance.Net.Interfaces;
using Binebase.Exchange.Common.Application;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Infrastructure;
using Binebase.Exchange.Common.Infrastructure.Services;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Application.Services;
using Binebase.Exchange.Gateway.Infrastructure.Identity;
using Binebase.Exchange.Gateway.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using Binebase.Exchange.Gateway.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Binebase.Exchange.Gateway.Infrastructure.Persistence;

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

                        services.AddTransient<IDateTime, DateTimeService>();
                        services.AddSingleton<ICacheClient, RedisCacheClient>();
                        services.AddTransient<IExchangeRateService, ExchangeRateService>();
                        services.AddTransient<IExchangeRateProvider, ExchangeRateProvider>();

                        services.AddSingleton<IBinanceSocketClient, BinanceSocketClient>();
                        services.AddTransient<IBinanceClient, BinanceClient>();

                        services.AddTransient<IIdentityService, IdentityService>();
                        services.AddTransient<ITransactionsSyncService, TransactionsSyncService>();
                        services.AddHttpClient<ICryptoService, CryptoService>().AddPolicyHandler(CommonInfrastructure.GetRetryPolicy());
                        services.AddConfigurationProviders(hostContext.Configuration);
                        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

                        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(x => x.SignIn.RequireConfirmedEmail = false)
                                 .AddDefaultTokenProviders()
                                 .AddEntityFrameworkStores<ApplicationDbContext>();

                    });
        }
    }
}
