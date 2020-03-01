using Binebase.Exchange.Common.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Binebase.Exchange.Gateway.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            
            services.AddDbContext<DbContext>(x => x.UseSqlServer(configuration.GetConnectionString("PersistenceConnection"), b => b.MigrationsAssembly(typeof(DbContext).Assembly.FullName)));
                
            services.AddServices(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
