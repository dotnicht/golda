using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Binebase.Exchange.Common.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Binebase.Exchange.Common.Worker
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddConfigurationProviders(hostContext.Configuration);
                });
    }
}
