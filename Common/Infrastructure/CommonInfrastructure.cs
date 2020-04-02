﻿using Binebase.Exchange.Common.Application;
using Binebase.Exchange.Common.Infrastructure.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.Slack;
using System;
using System.IO;
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
                    if (item.IsGenericType && item.GetGenericTypeDefinition() == typeof(IHttpClientScoped<>))
                    {
                        method.MakeGenericMethod(item.GetGenericArguments().Single(), type).Invoke(null, new[] { services });
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

        public static async Task<TResponse> Post<TRequest, TResponse>(this HttpClient target, string path, TRequest request) where TRequest : IRequest<TResponse>
        {
            var response = await target.PostAsync(path, new StringContent(JsonConvert.SerializeObject(request)));
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(content);
        }

        public static void ConfigureLogging(IConfiguration configuration, IHostEnvironment environment)
        {
            LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
                 .Enrich.FromLogContext()
                 .Enrich.WithExceptionDetails()
                 .Enrich.WithMachineName()
                 .WriteTo.Debug()
                 .WriteTo.Console()
                 .WriteTo.Slack(new SlackSinkOptions
                 {
                     WebHookUrl = "https://hooks.slack.com/services/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",
                     CustomChannel = "#errors",
                     BatchSizeLimit = 20,
                     CustomIcon = ":ghost:",
                     Period = TimeSpan.FromSeconds(10),
                     ShowDefaultAttachments = false,
                     ShowExceptionAttachments = true,   
                     MinimumLogEventLevel = Serilog.Events.LogEventLevel.Warning
                 })
                 .WriteTo.File(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) + $"\\logs\\{DateTime.UtcNow:yyyyMMddHHmm}log.log")
                 .Enrich.WithProperty("Environment", $"{environment.EnvironmentName}: {Assembly.GetCallingAssembly().GetName().Name}")
                 .ReadFrom.Configuration(configuration);

            if (environment.IsProduction())
            {
                loggerConfiguration.WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment.EnvironmentName));
            }

            Log.Logger = loggerConfiguration.CreateLogger();
        }

        private static ElasticsearchSinkOptions ConfigureElasticSink(IConfiguration configuration, string environment)
        {
            return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"{Assembly.GetCallingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
            };
        }
    }
}
