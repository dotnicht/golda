using Binebase.Exchange.Common.Application;
using Binebase.Exchange.Common.Infrastructure;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Binebase.Exchange.CryptoService.Infrastructure.Persistence;
using Binebase.Exchange.CryptoService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Binebase.Exchange.CryptoService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCommonInfrastructure(configuration);
            services.AddDbContext<ApplicationDbContext>(x => x.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            services.AddServices(Assembly.GetExecutingAssembly());
            services.AddHttpClient<IAccountService, AccountService>();
            return services;
        }
    }
}
