using AutoMapper;
using Binebase.Exchange.Common.Application.Behaviours;
using Binebase.Exchange.Common.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Binebase.Exchange.Common.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurationProviders(this IServiceCollection services, IConfiguration configuration)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var mi = typeof(OptionsConfigurationServiceCollectionExtensions)
                .GetMethod(nameof(OptionsConfigurationServiceCollectionExtensions.Configure), 1, new[] { typeof(IServiceCollection), typeof(IConfigurationSection) });

            foreach (var service in services.ToArray())
            {
                if (service.ImplementationType != null) // TODO: add support for factory initialization.
                {
                    foreach (var cfg in service.ImplementationType.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IConfigurationProvider<>)))
                    {
                        var type = cfg.GetGenericArguments().Single();
                        mi.MakeGenericMethod(type).Invoke(null, new object[] { services, configuration.GetSection($"{type.DeclaringType.Name}.{type.Name}") });
                    }
                }
            }

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services, Assembly assembly)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (assembly is null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            foreach (var type in assembly.GetExportedTypes())
            {
                foreach (var item in type.GetInterfaces())
                {
                    if (item.IsGenericType)
                    {
                        if (item.GetGenericTypeDefinition() == typeof(ITransient<>))
                        {
                            services.AddTransient(item.GenericTypeArguments.Single(), type);
                        }

                        if (item.GetGenericTypeDefinition() == typeof(ISingleton<>))
                        {
                            services.AddSingleton(item.GenericTypeArguments.Single(), type);
                        }

                        if (item.GetGenericTypeDefinition() == typeof(IScoped<>))
                        {
                            services.AddScoped(item.GenericTypeArguments.Single(), type);
                        }
                    }
                }
            }

            return services;
        }

        public static IServiceCollection AddApplicationCommon(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            return services;
        }
    }
}
