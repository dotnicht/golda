using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Common.Infrastructure.Interfaces;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Binebase.Exchange.CryptoService.Domain.Entities;
using Binebase.Exchange.CryptoService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using Nethereum.Web3;
using Newtonsoft.Json;
using QBitNinja.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Transaction = Binebase.Exchange.CryptoService.Domain.Entities.Transaction;

namespace Binebase.Exchange.CryptoService.Infrastructure.Services
{
    public class TransactionService : ITransactionService, IHttpClientScoped<ITransactionService>
    {
        private readonly Configuration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly IAccountService _accountService;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public TransactionService(IOptions<Configuration> options, IServiceProvider serviceProvider, IAccountService accountService, ILogger<TransactionService> logger, HttpClient httpClient)
            => (_configuration, _serviceProvider, _accountService, _logger, _httpClient) = (options.Value, serviceProvider, accountService, logger, httpClient);

        public async Task Subscribe(Currency currency, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var context = _serviceProvider.GetRequiredService<IApplicationDbContext>();

                    var addresses = context.Addresses
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
            var value = (ulong)balance.Operations.Sum(x => x.Amount.Satoshi);

            if (value > address.Balance)
            {
                address.Balance = value;
            }

            return balance.Operations.Select(x => new Transaction
            {
                AddressId = address.Id,
                Direction = TransactionDirection.Inbound,
                Confirmed = x.FirstSeen.DateTime,
                Hash = x.TransactionId.ToString(),
                Block = (ulong)x.Height,
                RawAmount = (ulong)x.Amount.Satoshi,
                Amount = x.Amount.ToDecimal(MoneyUnit.BTC)
            }).ToArray();
        }

        private async Task<Transaction[]> GetEthereumTransactions(Address address)
        {
            var result = new List<Transaction>();

            foreach (var operation in new[] { "balance", "txlist", "txlistinternal" })
            {
                var uri = string.Format(_configuration.EtherscanUriFormat,
                    _configuration.IsTestNet ? "ropsten" : "api",
                    $"{operation}&address={address.Public}");

                var response = await _httpClient.GetAsync(uri);
                var content = await response.Content.ReadAsStringAsync();

                if (operation == "balance")
                {
                    var balance = JsonConvert.DeserializeObject<EtherscanBalanceResponse>(content);
                    if (balance.Result > address.Balance)
                    {
                        address.Balance = balance.Result;
                        continue;
                    }

                    break;
                }

                var tx = JsonConvert.DeserializeObject<EtherscanTransactionsResponse>(content).Result
                    .Select(x => new Transaction
                    {
                        AddressId = address.Id,
                        Direction = TransactionDirection.Inbound,
                        Confirmed = DateTimeOffset.FromUnixTimeSeconds(x.TimeStamp).UtcDateTime,
                        Hash = x.Hash,
                        Block = x.BlockNumber,
                        RawAmount = x.Value,
                        Amount = Web3.Convert.FromWei(x.Value)
                    });

                result.AddRange(tx);
            }

            return result.ToArray();
        }

        private class EtherscanBalanceResponse
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public ulong Result { get; set; }
        }

        private class EtherscanTransactionsResponse
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public Transaction[] Result { get; set; }

            public class Transaction
            {
                public ulong BlockNumber { get; set; }
                public long TimeStamp { get; set; }
                public string Hash { get; set; }
                public string BlockHash { get; set; }
                public uint TransactionIndex { get; set; }
                public string From { get; set; }
                public string To { get; set; }
                public ulong Value { get; set; }
                public ulong Confirmations { get; set; }
            }
        }
    }
}
