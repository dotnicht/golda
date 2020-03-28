using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Crypto.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IDateTime _dateTime;
        private readonly IServiceProvider _services;

        public Worker(ILogger<Worker> logger, IDateTime dateTime, IServiceProvider services) 
            => (_logger, _dateTime, _services) = (logger, dateTime, services);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITransactionService>();
            var tasks = new[] { service.Subscribe(Currency.BTC, stoppingToken), service.Subscribe(Currency.ETH, stoppingToken) };
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("Worker running at: {time}", _dateTime.UtcNow);
                await Task.Delay(1000, stoppingToken);
            }

            Task.WaitAll(tasks);
        }
    }
}
