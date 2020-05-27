using Binebase.Exchange.Common.Application;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Application.Interfaces;
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
        private readonly ILogger _logger;
        private readonly IEnumerable<IBlockchainService> _blockchainServices;

        public TransactionService(
            IOptions<Configuration> options,
            IServiceProvider serviceProvider,
            ILogger<TransactionService> logger,
            IEnumerable<IBlockchainService> blockchainServices)
            => (_configuration, _serviceProvider, _logger, _blockchainServices) = (options.Value, serviceProvider, logger, blockchainServices);

        public async Task Subscribe(Currency currency, CancellationToken cancellationToken)
        {
            var service = _blockchainServices.Single(x => x.Currency == currency);
            var context = _serviceProvider.GetRequiredService<IApplicationDbContext>();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var addresses = context.Addresses
                        .Include(x => x.Transactions)
                        .Where(x => x.Currency == currency)
                        .ToArray();

                    using (new ElapsedTimer(_logger, "AddNewTx"))
                    {
                        foreach (var address in addresses.Where(x => x.Type == AddressType.Deposit && x.Index != _configuration.WithdrawAccountIndex))
                        {
                            _logger.LogDebug("Processing {currency} address {address}. Account Id {accountId}.", currency, address.Public, address.AccountId);

                            foreach (var tx in await service.GetTransactions(address.Public))
                            {
                                var existing = address.Transactions.Where(x => x.Hash == tx.Hash);
                                if (!existing.Any())
                                {
                                    tx.AddressId = address.Id;
                                    context.Transactions.Add(tx);
                                    _logger.LogDebug("Adding transaction {hash}. Generated Id {id}.", tx.Hash, tx.Id);
                                }
                            }

                            await context.SaveChangesAsync();
                        }
                    }

                    using (new ElapsedTimer(_logger, "UpdateExistingTx"))
                    {
                        var txs = addresses
                            .Where(x => x.Type == AddressType.Withdraw && x.Type == AddressType.Deposit)
                            .SelectMany(x => x.Transactions)
                            .Where(x => x.Status == TransactionStatus.Published);

                        foreach (var tx in txs)
                        {
                            var updated = await service.GetTransaction(tx.Hash);

                            tx.Status = updated.Status;
                            tx.Confirmed = updated.Confirmed;
                            tx.Confirmations = updated.Confirmations;

                            _logger.LogDebug("Updating transaction {hash}. Id {id}.", tx.Hash, tx.Id); await context.SaveChangesAsync();
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

        public async Task<decimal> TransferAssets(Currency currency)
        {
            var service = _blockchainServices.Single(x => x.Currency == currency);
            var context = _serviceProvider.GetRequiredService<IApplicationDbContext>();

            var indexes = context.Addresses
                .Where(x => x.Index != _configuration.WithdrawAccountIndex && x.Currency == currency && x.Type == AddressType.Deposit)
                .ToArray();

            if (await service.ValidateAddress(_configuration.TransferAddresses[currency]))
            {
                var txs = await service.TransferAssets(indexes, _configuration.TransferAddresses[currency]);

                context.Transactions.AddRange(txs);
                await context.SaveChangesAsync();

                return txs.Sum(x => x.Amount);
            }

            throw new InvalidOperationException();
        }
    }
}