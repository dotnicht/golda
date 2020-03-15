using Binebase.Exchange.Common.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Common.Worker
{
    public sealed class Worker : BackgroundService, IConfigurationProvider<Worker.Configuration>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Worker> _logger;
        private readonly Configuration _configuration;

        private List<Timer> _timers;

        public Worker(ILogger<Worker> logger, IOptions<Configuration> options, IServiceProvider serviceProvider)
            => (_logger, _configuration, _serviceProvider) = (logger, options.Value, serviceProvider);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timers = _configuration.Tasks.Select(x => new Timer(ExecuteInternal)).ToList(); // TODO: schedule.

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        private void ExecuteInternal(object state)
        {
            if (state is Configuration.Task task && _serviceProvider.GetRequiredService(Type.GetType(task.Type)) is ITask implementation)
            {
                var watch = Stopwatch.StartNew();
                try
                {
                    var result = implementation.Execute().Result;
                    watch.Stop();
                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"Task successfully executed. Elapsed {watch.Elapsed}.");
                    }
                    else
                    {
                        _logger.LogWarning($"Task execution failed. Execution result {string.Join(Environment.NewLine, result.Errors)}. Elapsed {watch.Elapsed}.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while executing task of type {task.Type}. Elapsed {watch.Elapsed}.");
                }
            }
            else
            {
                _logger.LogCritical("Configuration invalid.");
            }
        }

        public override void Dispose()
        {
            _timers.ForEach(x => x.Dispose());
            base.Dispose();
        }

        public class Configuration
        {
            public Dictionary<Guid, Task> Tasks { get; set; }

            public class Task
            {
                public string Type { get; set; }
            }
        }
    }
}
