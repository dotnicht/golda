using Binebase.Exchange.Common.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Binebase.Exchange.Common.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCommonInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddServices(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
