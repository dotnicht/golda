using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Binebase.Exchange.CryptoService.Domain.Entities;
using Binebase.Exchange.CryptoService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Infrastructure.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly Configuration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly IAccountService _accountService;
        private readonly ILogger _logger;
        private readonly IEnumerable<IBlockchainService> _blockchainServices;

        public TransactionService(
            IOptions<Configuration> options,
            IServiceProvider serviceProvider,
            IAccountService accountService,
            ILogger<TransactionService> logger,
            IEnumerable<IBlockchainService> blockchainServices)
            => (_configuration, _serviceProvider, _accountService, _logger, _blockchainServices) = (options.Value, serviceProvider, accountService, logger, blockchainServices);

        public async Task Subscribe(Currency currency, AddressType type, CancellationToken cancellationToken)
        {
            var service = _blockchainServices.Single(x => x.Currency == currency);
            var context = _serviceProvider.GetRequiredService<IApplicationDbContext>();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var addresses = context.Addresses
                        .Include(x => x.Transactions)
                        .Where(x => x.Currency == currency && x.Type == AddressType.Deposit)
                        .ToArray();

                    foreach (var address in addresses)
                    {
                        _logger.LogDebug("Processing {currency} address {address}. Account Id {accountId}.", currency, address.Public, address.AccountId);
                        var txs = new List<Transaction>();

                        foreach (var tx in await service.GetTransactions(address.Public))
                        {
                            if (address.Transactions.All(x => x.Hash != tx.Hash))
                            {
                                tx.AddressId = address.Id;
                                txs.Add(context.Transactions.Add(tx).Entity);
                            }
                        }

                        await context.SaveChangesAsync();

                        if (_configuration.DebitDepositTransactions)
                        {
                            foreach (var tx in txs)
                            {
                                await _accountService.Debit(address.AccountId, currency, tx.Amount, tx.Id);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Subscription to {currency} blockchain failed.", currency);
                }

                await Task.Delay(_configuration.TransactionPoolingTimeout);
            }
        }
    }
}
