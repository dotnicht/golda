using Binebase.Exchange.AccountService.Domain.Aggregates;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NEventStore;
using NEventStore.Domain;
using NEventStore.Domain.Core;
using NEventStore.Domain.Persistence;
using NEventStore.Domain.Persistence.EventStore;
using NEventStore.Logging;
using NEventStore.Persistence.Sql.SqlDialects;
using NEventStore.Serialization.Json;

namespace Binebase.Exchange.AccountService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            /*
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
            */

            if (environment.IsEnvironment("Test"))
            {
            }
            else
            {
                services.AddTransient<IDateTime, DateTimeService>();
            }

            //services.AddAuthentication();

            services.AddSingleton(x => Wireup
                .Init()
                .LogToOutputWindow(LogLevel.Verbose)
                .LogToConsoleWindow(LogLevel.Verbose)
                //.UsingInMemoryPersistence()
                .UsingSqlPersistence(SqlClientFactory.Instance, configuration.GetConnectionString("EventStoreConnection"))
                .WithDialect(new MsSqlDialect())
                .InitializeStorageEngine()
                //.Compress()
                //.EncryptWith()
                //.HookIntoPipelineUsing(new[] { new PipelineHook() })
                .UsingJsonSerialization()
                .Build());

            services.AddTransient<IRepository, EventStoreRepository>();
            services.AddTransient<IConstructAggregates, AggregateFactory>();
            services.AddTransient<IDetectConflicts, ConflictDetector>();

            return services;
        }
    }
}
