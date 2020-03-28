using Binebase.Exchange.Common.Application;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Infrastructure.Services;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Binebase.Exchange.CryptoService.Infrastructure;
using Binebase.Exchange.CryptoService.Infrastructure.Persistence;
using Binebase.Exchange.CryptoService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
                    services.AddTransient<IDateTime, DateTimeService>();
                    services.AddTransient<ITransactionService, TransactionService>();
                    //services.AddTransient<IAccountService, AccountService>();
                    services.AddHttpClient<IAccountService, AccountService>();
                    services.AddDbContext<ApplicationDbContext>(x => x.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
                    services.AddScoped<IApplicationDbContext>(x => x.GetRequiredService<ApplicationDbContext>());
                    //services.AddInfrastructure(hostContext.Configuration);
                    services.AddConfigurationProviders(hostContext.Configuration);
                    services.Configure<AccountService.Configuration>(hostContext.Configuration.GetSection("AccountService.Configuration"));
                });
    }
}
