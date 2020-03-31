using Binebase.Exchange.Common.Application;
using Binebase.Exchange.Common.Infrastructure.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Binebase.Exchange.Common.Infrastructure
{
    public static class CommonInfrastructure
    {
        public const string DecimalFormat = "decimal(18,8)"; 

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

        public static async Task<TResponse> Get<TResponse>(this HttpClient target, Uri source)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var response = await target.GetAsync(source);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(content);
        }

        public static Task<TResponse> Get<TRequest, TResponse>(this HttpClient source, string path, TRequest request) where TRequest : IRequest<TResponse>
        {
            throw new NotImplementedException();
        }
    }
}
