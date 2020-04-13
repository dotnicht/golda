using Binance.Net;
using Binance.Net.Interfaces;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Infrastructure;
using Binebase.Exchange.Common.Infrastructure.Services;
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
                        services.AddCommonInfrastructure(hostContext.Configuration);
                        services.AddHostedService<Worker>();

                        services.AddSingleton<IBinanceSocketClient, BinanceSocketClient>();
                        services.AddSingleton<ICacheClient, RedisCacheClient>();
                        services.AddSingleton<IExchangeRateProvider, ExchangeRateProvider>();

                        services.AddTransient<IDateTime, DateTimeService>();
                        services.AddTransient<IExchangeRateService, ExchangeRateService>();
                        services.AddTransient<IBinanceClient, BinanceClient>();

                        services.AddTransient<ITransactionService, TransactionService>();
                        services.AddHttpClient<ICryptoService, CryptoService>().AddRetryPolicy();

                        services.AddDbContext<ApplicationDbContext>(x => x.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection"),
                             b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

                        services.AddTransient<ICurrentUserService, SystemUserService>();
                        services.AddTransient<IApplicationDbContext, ApplicationDbContext>();
                        services.AddTransient<IUserContext, ApplicationDbContext>();

                        services.Configure<Crypto>(hostContext.Configuration.GetSection("Infrastructure.Crypto"));
                    });
        }
    }
}
