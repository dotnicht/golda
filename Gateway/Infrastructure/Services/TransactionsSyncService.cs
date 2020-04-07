using Binebase.Exchange.Gateway.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Binebase.Exchange.Common.Application.Interfaces;
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
            using var ctx = _serviceProvider.GetRequiredService<IUserContext>();
            var usersIds = ctx.Users.Select(x => x.Id).ToArray();

            foreach (var userId in usersIds)
            {
                var userTransactions = _cryptoService.GetTransactions(userId);
                UpdateTransactionsInStore(userTransactions.Result.ToList());
            }

            await Task.CompletedTask;
        }

        private void UpdateTransactionsInStore(List<Transaction> userTransactions)
        {
            using var scope = _serviceProvider.CreateScope();
            using var ctx = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
            foreach (var inTransaction in userTransactions)
            {
                var inputHash = inTransaction.GetHashCode();
                var existingTrans = ctx.Transactions.FirstOrDefault(t => t.Id == inTransaction.Id);
                if (existingTrans != null)
                {
                    if (!existingTrans.GetHashCode().Equals(inTransaction.GetHashCode()))
                    {
                        ctx.Transactions.Update(inTransaction);
                    }
                }
                else
                {
                    ctx.Transactions.AddAsync(inTransaction);
                }
            }
        }

        public class Configuration
        {
            public string SomeConfig { get; set; }
        }
    }
}
