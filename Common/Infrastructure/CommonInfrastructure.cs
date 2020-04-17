using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.Slack;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Binebase.Exchange.Common.Infrastructure
{
    public static class CommonInfrastructure
    {
        public const string DecimalFormat = "decimal(18,8)";

        public static IServiceCollection AddCommonInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddTransient<IDateTime, DateTimeService>();
        }

        public static IHttpClientBuilder AddRetryPolicy(this IHttpClientBuilder httpClientBuilder)
        {
            if (httpClientBuilder is null)
            {
                throw new ArgumentNullException(nameof(httpClientBuilder));
            }

            // TODO: add policy to config.
            httpClientBuilder.AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                .WaitAndRetryAsync(new[]
                    {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(3)
                    },
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        Log.Logger.Warning("Delaying for {delay}, then making retry {retry}.", timespan, retryAttempt);
                    }));

            return httpClientBuilder;
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

        public static void ConfigureLogging(IConfiguration configuration, IHostEnvironment environment, bool slack = true)
        {
            var loggerConfiguration = new LoggerConfiguration()
                  .Enrich.FromLogContext()
                  .Enrich.WithExceptionDetails()
                  .Enrich.WithMachineName()
                  .WriteTo.Debug()
                  .WriteTo.Console()
                  .Enrich.WithProperty("Environment", $"{environment.EnvironmentName}: {Assembly.GetCallingAssembly().GetName().Name}")
                  .ReadFrom.Configuration(configuration);

            if (environment.IsProduction())
            {
                loggerConfiguration
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
                    {
                        ModifyConnectionSettings = x => x.BasicAuthentication("elastic", "Binebase123"),
                        AutoRegisterTemplate = true,
                        IndexFormat = $"{Assembly.GetEntryAssembly().GetName().Name.ToLower().Replace(".", "-")}"
                    });

                if (slack)
                {
                    loggerConfiguration
                        .WriteTo.Slack(new SlackSinkOptions
                        {
                            WebHookUrl = "https://hooks.slack.com/services/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",
                            CustomChannel = "#errors",
                            BatchSizeLimit = 20,
                            CustomIcon = ":ghost:",
                            Period = TimeSpan.FromSeconds(10),
                            ShowDefaultAttachments = false,
                            ShowExceptionAttachments = true,
                            MinimumLogEventLevel = LogEventLevel.Error
                        });
                }
            }
            else
            {
                loggerConfiguration.WriteTo.File(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) + $"\\logs\\{DateTime.UtcNow:yyyyMMdd}.log");
            }

            Log.Logger = loggerConfiguration.CreateLogger();
        }
    }
}