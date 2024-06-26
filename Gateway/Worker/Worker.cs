using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Infrastructure.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IDateTime _dateTime;
        private readonly IExchangeRateService _exchangeRateService;
        private readonly ITransactionService _transactionsSyncService;
        private readonly IAggregateService _aggregateService;
        public Worker(ILogger<Worker> logger, IDateTime dateTime, IExchangeRateService exchangeRateService, ITransactionService transactionsSyncService, IAggregateService aggregateService)
            => (_logger, _dateTime, _exchangeRateService, _transactionsSyncService, _aggregateService) = (logger, dateTime, exchangeRateService, transactionsSyncService, aggregateService);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tasks = new[]
            {
                _exchangeRateService.Subscribe(),
                _transactionsSyncService.SyncTransactions(stoppingToken),
                _aggregateService.PopulateBalances(stoppingToken)
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("Worker running at: {time}", _dateTime.UtcNow);
                await Task.Delay(1000, stoppingToken);
            }

            await Task.WhenAll(tasks);
        }
    }
}
