using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Binebase.Exchange.CryptoService.Domain.Entities;
using Binebase.Exchange.CryptoService.Domain.Enums;
using EtherscanApi.Net.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using QBitNinja.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Transaction = Binebase.Exchange.CryptoService.Domain.Entities.Transaction;

namespace Binebase.Exchange.CryptoService.Infrastructure.Services
{
    public class TransactionService : ITransactionService, IConfigurationProvider<TransactionService.Configuration>, ITransient<ITransactionService>
    {
        private readonly Configuration _configuration;
        private readonly IApplicationDbContext _context;
        private readonly IAccountService _accountService;
        private readonly ILogger _logger;

        public TransactionService(IOptions<Configuration> options, IApplicationDbContext context, IAccountService accountService, ILogger<TransactionService> logger)
            => (_configuration, _context, _accountService, _logger) = (options.Value, context, accountService, logger);

        public async Task Subscribe(Currency currency, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var addresses = _context.Addresses
                        .Include(x => x.Transactions)
                        .Where(x => x.Currency == currency && x.Type == AddressType.Deposit)
                        .ToArray();

                    foreach (var address in addresses)
                    {
                        _logger.LogDebug($"Processing {currency} address {address.Public}. Account Id {address.AccountId}.");
                        var txs = new List<Transaction>();

                        foreach (var tx in await GetTransactions(address))
                        {
                            if (address.Transactions.All(x => x.Hash != tx.Hash))
                            {
                                txs.Add(_context.Transactions.Add(tx).Entity);
                            }
                        }

                        await _context.SaveChangesAsync();

                        if (_configuration.DebitDepositTransactions)
                        {
                            foreach (var tx in txs)
                            {
                                await _accountService.Debit(address.AccountId, currency, tx.Amount, tx.Id);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, $"Subscription to {currency} blockchain failed.");
                }

                await Task.Delay(_configuration.TransactionPoolingTimeout);
            }
        }

        public async Task<Transaction[]> GetTransactions(Address address)
        {
            if (address is null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            return address.Currency switch
            {
                Currency.BTC => await GetBitcoinTransactions(address),
                Currency.ETH => await GetEthereumTransactions(address),
                _ => throw new NotSupportedException(),
            };
        }

        private async Task<Transaction[]> GetBitcoinTransactions(Address address)
        {
            var network = _configuration.IsTestNet ? Network.TestNet : Network.Main;
            var client = new QBitNinjaClient(network);
            var balance = await client.GetBalance(BitcoinAddress.Create(address.Public, network));
            var value = balance.Operations.Sum(x => x.Amount.Satoshi);

            if (value > address.Balance)
            {
                address.Balance = value;
            }

            return balance.Operations.Select(x => new Transaction
            {
                Address = address,
                AddressId = address.Id,
                Direction = TransactionDirection.Inbound,
                Confirmed = x.FirstSeen.DateTime,
                Hash = x.TransactionId.ToString(),
                Block = (ulong)x.Height,
                RawAmount = x.Amount.Satoshi,
                Amount = x.Amount.ToDecimal(MoneyUnit.BTC)
            }).ToArray();
        }

        private async Task<Transaction[]> GetEthereumTransactions(Address address)
        {
            var client = new EtherScanClient(_configuration.EtherscanApiKey);
            //var balance = client.GetEtherBalance(address.Public).Result;

            //if (balance > address.Balance)
            //{
            //    address.Balance = balance;
            //}

            var tx = client.GetTransactions(address.Public).Result;

            return tx.Select(x => new Transaction
            {
                Address = address, 
                AddressId = address.Id,
                Direction = TransactionDirection.Inbound,
                //Confirmed = x.
                Hash = x.TxId,
                Block = x.BlockNumber,
                RawAmount = Nethereum.Web3.Web3.Convert.ToWei(x.Value),
                Amount = x.Value
            }).ToArray();
        }

        public class Configuration
        {
            public bool IsTestNet { get; set; }
            public bool DebitDepositTransactions { get; set; }
            public TimeSpan TransactionPoolingTimeout { get; set; }
            public string EtherscanApiKey { get; set; } // QJZXTMH6PUTG4S3IA4H5URIIXT9TYUGI7P
        }
    }
}
