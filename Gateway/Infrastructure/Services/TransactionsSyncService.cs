using Binebase.Exchange.Gateway.Application.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using Binebase.Exchange.Gateway.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Binebase.Exchange.Gateway.Infrastructure.Interfaces;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Binebase.Exchange.Gateway.Infrastructure.Services
{
    public class TransactionsSyncService : ITransactionsSyncService
    {
        private readonly Configuration _configuration;
        private readonly ILogger _logger;
        private readonly ICryptoService _cryptoService;
        private readonly IServiceProvider _serviceProvider;

        public TransactionsSyncService(IOptions<Configuration> options, ILogger<TransactionsSyncService> logger, ICryptoService cryptoService, IServiceProvider serviceProvider) =>
             (_configuration, _logger, _cryptoService, _serviceProvider) = (options.Value, logger, cryptoService, serviceProvider);

        public async Task SyncTransactions()
        {
            using var scope = _serviceProvider.CreateScope();
            using var ctx = scope.ServiceProvider.GetRequiredService<IUserContext>();

            Guid[] usersIds = ctx.Users.Select(x => x.Id).ToArray();

            foreach (var userId in usersIds)
            {
                var userTransactions = await _cryptoService.GetTransactions(userId);
                await UpdateTransactionsInStore(userTransactions);
            }

            await Task.CompletedTask;
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
                        ctx.Transactions.Update(inTransaction);
                    }
                }
                else
                {
                    await ctx.Transactions.AddAsync(inTransaction);
                }
            }
            await ctx.SaveChangesAsync();
        }

        public class Configuration
        {
            public string SomeConfig { get; set; }
        }
    }
}
