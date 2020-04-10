using Binebase.Exchange.AccountService.Domain.Aggregates;
using Binebase.Exchange.Common.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCommonInfrastructure(configuration);

            // TODO: wrap event store into singleton service.
            services.AddSingleton(x => Wireup
                .Init()
                .LogToOutputWindow(LogLevel.Info)
                .LogToConsoleWindow(LogLevel.Info)
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
