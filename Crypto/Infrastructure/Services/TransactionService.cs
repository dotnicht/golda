using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Binebase.Exchange.CryptoService.Domain.Entities;
using Binebase.Exchange.CryptoService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NBitcoin;
using QBitNinja.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Transaction = Binebase.Exchange.CryptoService.Domain.Entities.Transaction;

namespace Binebase.Exchange.CryptoService.Infrastructure.Services
{
    public class TransactionService : ITransactionService, IConfigurationProvider<TransactionService.Configuration>, ITransient<ITransactionService>
    {
        private readonly Configuration _configuration;
        private readonly IApplicationDbContext _context;

        public TransactionService(IOptions<Configuration> options, IApplicationDbContext context)
            => (_configuration, _context) = (options.Value, context);

        public async Task Subscribe(Currency currency, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                foreach (var address in _context.Addresses.Include(x => x.Transactions).Where(x => x.Currency == currency && x.Type == AddressType.Deposit))
                {
                    foreach (var tx in await GetTransactions(address))
                    {
                        if (address.Transactions.All(x => x.Hash != tx.Hash))
                        {
                            _context.Transactions.Add(tx);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
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
                AddressId = address.Id,
                Direction = TransactionDirection.Inbound,
                Confirmed = x.FirstSeen.DateTime,
                Hash = x.TransactionId.ToString(),
                Block = x.Height,
                Amount = x.Amount.Satoshi,
            }).ToArray();
        }

        private async Task<Transaction[]> GetEthereumTransactions(Address address)
        {
            throw new NotImplementedException();
        }

        public class Configuration
        {
            public bool IsTestNet { get; set; }
            public TimeSpan TransactionPoolingTimeout { get; set; }
        }
    }
}
