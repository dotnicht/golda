using Binance.Net;
using Binance.Net.Interfaces;
using Binebase.Exchange.Common.Application;
using Binebase.Exchange.Common.Infrastructure;
using Binebase.Exchange.Gateway.Application.Configuration;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Application.Services;
using Binebase.Exchange.Gateway.Infrastructure.Configuration;
using Binebase.Exchange.Gateway.Infrastructure.Interfaces;
using Binebase.Exchange.Gateway.Infrastructure.Persistence;
using Binebase.Exchange.Gateway.Infrastructure.Services;
using Binebase.Exchange.Gateway.Worker;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
                    var configuration = hostContext.Configuration;

                    services.AddDbContext<ApplicationDbContext>(x => x.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

                    services.AddCommonInfrastructure(configuration);

                    services.AddSingleton<IExchangeRateProvider, ExchangeRateProvider>();
                    services.AddSingleton<ICacheClient, RedisCacheClient>();
                    services.AddSingleton<IBinanceSocketClient, BinanceSocketClient>();

                    services.AddTransient<ITransactionService, TransactionService>();
                    services.AddTransient<IEmailService, EmailService>();
                    services.AddTransient<IAggregateService, AggregateService>();
                    services.AddTransient<IExchangeRateService, ExchangeRateService>();

                    services.AddHttpClient<IAccountService, AccountService>().AddRetryPolicy();
                    services.AddHttpClient<ICryptoService, CryptoService>().AddRetryPolicy();

                    services.Configure<Account>(configuration.GetSection("Infrastructure.Account"));
                    services.Configure<Crypto>(configuration.GetSection("Infrastructure.Crypto"));
                    services.Configure<Email>(configuration.GetSection("Infrastructure.Email"));
                    services.Configure<Redis>(configuration.GetSection("Infrastructure.Redis"));
                    services.Configure<Aggregation>(configuration.GetSection("Infrastructure.Aggregation"));
                    services.Configure<ExchangeRates>(configuration.GetSection("Application.ExchangeRates"));

                    services.AddCommonApplication();
                    services.AddHostedService<Worker>();

                    services.AddTransient<ICurrentUserService, SystemUserService>();
                    services.AddTransient<IInfrastructureContext, ApplicationDbContext>();
                    services.AddScoped<IApplicationDbContext>(x => x.GetRequiredService<ApplicationDbContext>());
                });
        }
    }
}
