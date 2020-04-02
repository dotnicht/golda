using Binebase.Exchange.AccountService.Application;
using Binebase.Exchange.AccountService.Contracts.Commands;
using Binebase.Exchange.AccountService.Infrastructure;
using Binebase.Exchange.Common.Api;
using Binebase.Exchange.Common.Application;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag;
using NSwag.Generation.Processors.Security;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Binebase.Exchange.AccountService.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            (Configuration, Environment) = (configuration, environment);
            ConfigureLogging(configuration, environment);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication();
            services.AddApplicationCommon();
            services.AddInfrastructure(Configuration);

            services.AddHttpContextAccessor();

            services.AddControllers()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<AddAssetCommand>()) // TODO: replace.
                .AddNewtonsoftJson();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddOpenApiDocument(configure =>
            {
                configure.Title = "Binebase.Exchange.AccountService API";

                configure.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}."
                });

                configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseSerilogRequestLogging();

            app.UseCustomExceptionHandler();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSwaggerUi3(settings =>
            {
                settings.Path = "/api";
                settings.DocumentPath = "/api/specification.json";
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller}/{action=Index}/{id?}");
            });
        }
        private static void ConfigureLogging(IConfiguration configuration, IWebHostEnvironment environment)
        {
            LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
                 .Enrich.FromLogContext()
                 .Enrich.WithExceptionDetails()
                 .Enrich.WithMachineName()
                 .WriteTo.Debug()
                 .WriteTo.Console()
                 .WriteTo.File(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $"\\logs\\{DateTime.UtcNow:yyyyMMddHHmm}log.log")
                 .Enrich.WithProperty("Environment", environment)
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
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
            };
        }
    }
}
