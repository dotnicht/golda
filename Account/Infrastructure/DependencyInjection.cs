using Binebase.Exchange.AccountService.Application;
using Binebase.Exchange.AccountService.Application.Common.Interfaces;
using Binebase.Exchange.AccountService.Domain.Aggregates;
using Binebase.Exchange.AccountService.Infrastructure.Identity;
using Binebase.Exchange.AccountService.Infrastructure.Persistence;
using Binebase.Exchange.AccountService.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
                services.AddTransient<IIdentityService, IdentityService>();
                services.AddTransient<IDateTime, DateTimeService>();
            }

            services.AddAuthentication();

            var store = Wireup
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
                .UsingJsonSerialization();

            services.AddSingleton(store.Build());

            services.AddTransient<IRepository, EventStoreRepository>();
            services.AddTransient<IConstructAggregates, AggregateFactory>();
            services.AddTransient<IDetectConflicts, ConflictDetector>();

            return services;
        }
    }
}
