using Binebase.Exchange.Common.Application;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Infrastructure.Configuration;
using Binebase.Exchange.Gateway.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Infrastructure.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly Crypto _configuration;
        private readonly ILogger _logger;
        private readonly ICryptoService _cryptoService;
        private readonly IServiceProvider _serviceProvider;

        public TransactionService(IOptions<Crypto> options, ILogger<TransactionService> logger, ICryptoService cryptoService, IServiceProvider serviceProvider) =>
             (_configuration, _logger, _cryptoService, _serviceProvider) = (options.Value, logger, cryptoService, serviceProvider);

        public async Task SyncTransactions(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using (new ElapsedTimer(_logger, "CryptoTxProcess"))
                    {
                        using var scope = _serviceProvider.CreateScope();
                        using var ctx = scope.ServiceProvider.GetRequiredService<IUserContext>();

                        var usersIds = ctx.Users.Select(x => x.Id).ToArray();

                        foreach (var userId in usersIds)
                        {
                            var userTransactions = await _cryptoService.GetTransactions(userId);
                            // TODO: update tx status.
                            await UpdateTransactionsInStore(userTransactions, userId);
                        }
                    }

                    await Task.Delay(_configuration.TransactionsSyncTimeout);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while sync transactions.");
                }
            }
        }

        private async Task UpdateTransactionsInStore(Transaction[] userTransactions, Guid userId)
        {
            using var scope = _serviceProvider.CreateScope();
            using var ctx = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
            foreach (var inTransaction in userTransactions)
            {
                inTransaction.CreatedBy = userId;
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
    }
}
