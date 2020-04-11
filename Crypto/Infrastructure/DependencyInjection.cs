using Binebase.Exchange.Common.Infrastructure;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Binebase.Exchange.CryptoService.Infrastructure.Persistence;
using Binebase.Exchange.CryptoService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Binebase.Exchange.CryptoService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCommonInfrastructure(configuration);
            services.AddDbContext<ApplicationDbContext>(x => x.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), 
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            services.AddScoped<IApplicationDbContext>(x => x.GetRequiredService<ApplicationDbContext>());
            services.AddHttpClient<IAccountService, AccountService>();
            services.AddHttpClient<IBlockchainService, EthereumService>();
            services.AddTransient<IBlockchainService, BitcoinService>();
            services.AddTransient<ITransactionService, TransactionService>();
            services.Configure<Configuration>(configuration.GetSection("Infrastructure.Configuration"));
            return services;
        }
    }
}
