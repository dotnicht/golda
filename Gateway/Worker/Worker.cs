using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Common.Application.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IDateTime _dateTime;
        private readonly IExchangeRateService _exchangeRateService;

        public Worker(ILogger<Worker> logger, IDateTime dateTime, IExchangeRateService exchangeRateService)
            => (_logger, _dateTime, _exchangeRateService) = (logger, dateTime, exchangeRateService);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tasks = new[] { _exchangeRateService.Subscribe() };

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("Worker running at: {time}", _dateTime.UtcNow);
                await Task.Delay(1000, stoppingToken);
            }

            Task.WaitAll(tasks);
        }
    }
}
