using Binebase.Exchange.Common.Infrastructure.Clients.Account;
using Binebase.Exchange.Gateway.Application;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly Account _configuration;
        private readonly AccountClient _accountClient;
        private readonly AssetClient _assetClient;

        public AccountService(HttpClient client, IOptions<Account> options)
            => (_configuration, _accountClient, _assetClient) 
                = (options.Value, new AccountClient(options.Value.Address.ToString(), client), new AssetClient(options.Value.Address.ToString(), client));

        public async Task CretateDefaultAccount(Guid id)
        {
            await _accountClient.NewAsync(new NewAccountCommand { Id = id });

            foreach (var currency in _configuration.Currencies)
            {
                await _assetClient.AssetAsync(new AddAssetCommand { Id = id, AssetId = Guid.NewGuid(), Currency = (Currency)currency });
            }
        }

        public async Task<decimal> GetBalance(Guid id, Common.Domain.Currency currency)
        {
            var portfolio = await GetPorfolio(id);
            if (portfolio.ContainsKey(currency))
            {
                return portfolio[currency];
            }

            throw new NotSupportedException(ErrorCode.CurrencyNotSupported);
        }

        public async Task<Dictionary<Common.Domain.Currency, decimal>> GetPorfolio(Guid id)
        {
            var portfolio = await _accountClient.PortfolioAsync(id);
            return portfolio.Portfolio.ToDictionary(k => (Common.Domain.Currency)k.Currency, k => k.Balance);
        }

        public async Task Debit(Guid id, Common.Domain.Currency currency, decimal amount, Guid externalId, Common.Domain.TransactionType type)
        {
            var portfolio = await _accountClient.PortfolioAsync(id);

            var cmd = new DebitCommand
            {
                Id = id,
                AssetId = portfolio.Portfolio.Single(x => x.Currency == (Currency)currency).Id,
                TransactionId = externalId,
                Amount = amount,
                Type = (TransactionType)type
            };

            await _assetClient.DebitAsync(cmd);
        }

        public async Task Credit(Guid id, Common.Domain.Currency currency, decimal amount, Guid externalId, Common.Domain.TransactionType type)
        {
            var portfolio = await _accountClient.PortfolioAsync(id);

            var cmd = new CreditCommand
            {
                Id = id,
                AssetId = portfolio.Portfolio.Single(x => x.Currency == (Currency)currency).Id,
                TransactionId = externalId,
                Amount = amount,
                Type = (TransactionType)type
            };

            await _assetClient.CreditAsync(cmd);
        }

        public async Task<Domain.Entities.Transaction[]> GetTransactions(Guid id)
        {
            var portfolio = await _accountClient.PortfolioAsync(id);
            var assets = portfolio.Portfolio.ToDictionary(x => x.Id, x => new Asset { Currency = x.Currency });
            var txs = await _accountClient.TransactionsAsync(id);
            var result = new List<Domain.Entities.Transaction>();

            foreach (var tx in txs.Transactions)
            {
                var item = new Domain.Entities.Transaction
                {
                    Id = tx.TransactionId,
                    DateTime = tx.DateTime.DateTime,
                    Amount = tx.Amount,
                    Currency = (Common.Domain.Currency)assets[tx.AssetId].Currency,
                    Type = (Common.Domain.TransactionType)tx.Type,
                };

                result.Add(item);
            }

            return result.ToArray();
        }
    }
}