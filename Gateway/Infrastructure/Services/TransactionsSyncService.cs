using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binebase.Exchange.Common.Infrastructure.Interfaces;

namespace Binebase.Exchange.Gateway.Infrastructure.Services
{
    public class TransactionsSyncService : ITransactionsSyncService, IHttpClientScoped<ITransactionsSyncService>
    {
        private readonly Configuration _configuration;
        private readonly ILogger _logger;
        private readonly ICryptoService _cryptoService;
        private readonly IServiceProvider _serviceProvider;

        public TransactionsSyncService(IOptions<Configuration> options, ILogger<TransactionsSyncService> logger, ICryptoService cryptoService, IServiceProvider serviceProvider) =>
             (_configuration, _logger, _cryptoService, _serviceProvider) = (options.Value, logger, cryptoService, serviceProvider);

        public async Task SyncTransactions(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Start transactions synchronizations task at: {DateTime.UtcNow}.");
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    using var ctx = scope.ServiceProvider.GetRequiredService<IUserContext>();

                    Guid[] usersIds = ctx.Users.Select(x => x.Id).ToArray();

                    foreach (var userId in usersIds)
                    {
                        //_logger.LogDebug($"Processing transactions for user with id{userId.ToString()}.");
                        var userTransactions = await _cryptoService.GetTransactions(userId);
                        await UpdateTransactionsInStore(userTransactions);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error while sync transactions.");
                }
            }
            _logger.LogDebug($"End transactions synchronizations task at: {DateTime.UtcNow}.");
            await Task.Delay(_configuration.TransactionsSyncTimeout);
        }

        private async Task UpdateTransactionsInStore(Transaction[] userTransactions)
        {
            using var scope = _serviceProvider.CreateScope();
            using var ctx = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
            foreach (var inTransaction in userTransactions)
            {
                var inputHash = inTransaction.GetHashCode();
                var existingTrans = ctx.Transactions.FirstOrDefault(t => t.Id == inTransaction.Id);
                if (existingTrans != null)
                {
                    if (existingTrans.Hash != inTransaction.Hash)
                    {
                        _logger.LogDebug($"Update in store transaction with id: {existingTrans.Id}.");
                        ctx.Transactions.Update(inTransaction);
                    }
                }
                else
                {
                    _logger.LogDebug($"Add to store transaction with id: {inTransaction.Id}.");
                    await ctx.Transactions.AddAsync(inTransaction);
                }
            }
            await ctx.SaveChangesAsync();
        }

        public class Configuration
        {
            public TimeSpan TransactionsSyncTimeout { get; set; }
        }
    }
}
