using Binebase.Exchange.Common.Application;
using Binebase.Exchange.Common.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Binebase.Exchange.Common.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCommonInfrastructure(this IServiceCollection services)
        {
            services.AddServices(Assembly.GetExecutingAssembly());
            return services;
        }

        public static IServiceCollection AddHttpClients(this IServiceCollection services, Assembly assembly)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (assembly is null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var method = typeof(HttpClientFactoryServiceCollectionExtensions)
                .GetMethod(nameof(HttpClientFactoryServiceCollectionExtensions.AddHttpClient), 2, new[] { typeof(IServiceCollection) });

            foreach (var type in assembly.GetExportedTypes())
            {
                foreach (var item in type.GetInterfaces())
                {
                    if (item.IsGenericType)
                    {
                        if (item.GetGenericTypeDefinition() == typeof(IHttpClientScoped<>))
                        {
                            method.MakeGenericMethod(item.GetGenericArguments().Single(), type).Invoke(null, new[] { services });
                        }
                    }
                }
            }

            return services;
        }
    }
}
