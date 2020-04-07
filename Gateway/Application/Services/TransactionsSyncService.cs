using Binebase.Exchange.Gateway.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Binebase.Exchange.Gateway.Application.Services
{
    public class TransactionsSyncService : ITransactionsSyncService, IConfigurationProvider<ExchangeRateService.Configuration>
    {
        private readonly Configuration _configuration;
        private readonly ILogger _logger;
        private readonly ICryptoService _cryptoService;
        private readonly IIdentityService _identityService;
        private readonly IApplicationDbContext _context;
        public TransactionsSyncService(IOptions<Configuration> options, ILogger<ExchangeRateService> logger, ICryptoService cryptoService, IIdentityService identityService, IApplicationDbContext context) =>
            (_configuration, _logger, _cryptoService, _identityService, _context) = (options.Value, logger, cryptoService, identityService, context);

        public async Task SyncTransactions()
        {
            var usersIds = _identityService.GetUsersIDs();

            foreach (var userId in usersIds)
            {
                var userTransactions = _cryptoService.GetTransactions(userId);
                UpdateTransactionsInStore(userTransactions.Result.ToList());
            }

            await Task.CompletedTask;
        }

        private void UpdateTransactionsInStore(List<Transaction> userTransactions)
        {
            foreach (var inTransaction in userTransactions)
            {
                var inputHash = inTransaction.GetHashCode();
                var existingTrans = _context.Transactions.FirstOrDefault(t => t.Id == inTransaction.Id);
                if (existingTrans != null)
                {
                    if (!existingTrans.GetHashCode().Equals(inTransaction.GetHashCode()))
                    {
                        _context.Transactions.Update(inTransaction);
                    }
                }
                else
                {
                    _context.Transactions.AddAsync(inTransaction);
                }
            }
        }

        public class Configuration
        {
            public string SomeConfig { get; set; }
        }
    }
}
