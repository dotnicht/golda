using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;
using System.IO;

namespace Binebase.Exchange.AccountService.Api
{
    public sealed class Program
    {
        public static void Main(string[] args) =>
            CreateWebHostBuilder(args).Build().Run();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
             .UseSerilog()
             .UseStartup<Startup>();


    }
}
