using Binebase.Exchange.Common.Infrastructure.Clients.Crypto;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TransactionType = Binebase.Exchange.Common.Domain.TransactionType;

namespace Binebase.Exchange.Gateway.Infrastructure.Services
{
    public class CryptoService : ICryptoService
    {
        private readonly Crypto _configuration;
        private readonly CryptoClient _cryptoClient;

        public CryptoService(HttpClient client, IOptions<Crypto> options)
        {
            _configuration = options.Value;
            client.BaseAddress = _configuration.Address;
            _cryptoClient = new CryptoClient(_configuration.Address.ToString(), client);
        }

        public async Task GenerateDefaultAddresses(Guid id)
        {
            foreach (var currency in _configuration.Currencies)
            {
                await GenerateAddress(id, currency);
            }
        }

        public async Task<string> GetAddress(Guid id, Common.Domain.Currency currency)
        {
            var addresses = await GetAddresses(id);
            return addresses.ContainsKey(currency) ? addresses[currency] : throw new NotSupportedException();
        }

        public async Task<Dictionary<Common.Domain.Currency, string>> GetAddresses(Guid id)
        {
            var result = await _cryptoClient.Addresses2Async(id);
            return result.Addresses.ToDictionary(x => (Common.Domain.Currency)x.Currency, x => x.Public);
        }

        public async Task<string> GenerateAddress(Guid id, Common.Domain.Currency currency)
        {
            var result = await _cryptoClient.AddressesAsync(new GenerateAddressCommand { Id = id, Currency = (Currency)currency });
            return result.Address;
        }

        public async Task<Domain.Entities.Transaction[]> GetTransactions(Guid id)
        {
            var txs = await _cryptoClient.Transactions2Async(id);
            var result = new List<Domain.Entities.Transaction>();

            foreach (var tx in txs.Transactions)
            {
                var item = new Domain.Entities.Transaction
                {
                    CreatedBy = id,
                    Id = tx.Id,
                    Currency = (Common.Domain.Currency)tx.Currency,
                    Amount = tx.Amount,
                    Hash = tx.Hash,
                    Failed = tx.Status == TransactionStatus.Failed,
                    Type = tx.Direction switch
                    {
                        TransactionDirection.Inbound => TransactionType.Deposit,
                        TransactionDirection.Outbound => TransactionType.Withdraw,
                        TransactionDirection.Internal => TransactionType.Internal,
                        _ => throw new InvalidOperationException(),
                    }
                };

                result.Add(item);
            }

            return result.ToArray();
        }

        public async Task<string> PublishTransaction(Guid id, Common.Domain.Currency currency, decimal amount, string address, Guid externalId)
        {
            var result = await _cryptoClient.TransactionsAsync(new PublishTransactionCommand { Id = id, Currency = (Currency)currency, Amount = amount, Public = address, ExternalId = externalId });
            return result.Hash;
        }

        public async Task Transfer()
        {
             await _cryptoClient.TransferAsync(new TransferDepositsCommand());        
        }
    }
}
