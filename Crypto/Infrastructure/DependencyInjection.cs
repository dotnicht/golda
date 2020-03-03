using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Infrastructure.Services;
using Binebase.Exchange.CryptoService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Binebase.Exchange.CryptoService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            services.AddDbContext<DbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(DbContext).Assembly.FullName)));

            services.AddScoped((System.Func<System.IServiceProvider, IDbContext>)(provider => provider.GetService<Persistence.ApplicationDbContext>()));

            if (environment.IsEnvironment("Test"))
            {

            }
            else
            {
                services.AddTransient<IDateTime, DateTimeService>();
            }

            return services;
        }
    }
}
