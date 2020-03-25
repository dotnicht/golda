using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Crypto.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IDateTime _dateTime;
        private readonly ITransactionService _transactionService;

        public Worker(ILogger<Worker> logger, IDateTime dateTime, ITransactionService transactionService) 
            => (_logger, _dateTime, _transactionService) = (logger, dateTime, transactionService);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tasks = new[] { _transactionService.Subscribe(Currency.BTC, stoppingToken), _transactionService.Subscribe(Currency.ETH, stoppingToken) };

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("Worker running at: {time}", _dateTime.UtcNow);
                await Task.Delay(1000, stoppingToken);
            }

            Task.WaitAll(tasks);
        }
    }
}
