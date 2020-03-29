using Binebase.Exchange.Gateway.Infrastructure.Persistence;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Api
{
    public sealed class Program
    {
        public async static Task Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var identity = services.GetRequiredService<IdentityDbContext>();
                    await identity.Database.MigrateAsync();
                    var context = services.GetRequiredService<Persistence.ApplicationDbContext>();
                    await context.Database.MigrateAsync();
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
            => WebHost.CreateDefaultBuilder(args)
                 .ConfigureLogging(logging =>
                 {
                     logging.ClearProviders();
                     logging.AddConsole();
                     logging.AddAzureWebAppDiagnostics();
                     logging.AddEventSourceLogger();
                 })
                .UseStartup<Startup>();
    }
}
