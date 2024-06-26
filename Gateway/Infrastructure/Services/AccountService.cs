﻿using Binebase.Exchange.Common.Infrastructure.Clients.Account;
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
        private readonly ICacheClient _cacheClient;

        public AccountService(HttpClient client, IOptions<Account> options, ICacheClient cacheClient)
            => (_configuration, _accountClient, _assetClient, _cacheClient) 
                = (options.Value, new AccountClient(options.Value.Address.ToString(), client), new AssetClient(options.Value.Address.ToString(), client), cacheClient);

        public async Task CretateDefaultAccount(Guid id)
        {
            await _accountClient.NewAsync(new NewAccountCommand { Id = id });

            foreach (var currency in _configuration.Currencies)
            {
                await _assetClient.AssetAsync(new AddAssetCommand { Id = id, AssetId = Guid.NewGuid(), Currency = (Currency)currency });
            }

            await GetPortfolioInternal(id);
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

        public async Task<Dictionary<Common.Domain.Currency, decimal>> GetPorfolio(Guid id, bool force = false)
        {
            var portfolio = await GetPortfolioInternal(id, force);
            return portfolio.Portfolio.ToDictionary(k => (Common.Domain.Currency)k.Currency, k => k.Balance);
        }

        public async Task Debit(Guid id, Common.Domain.Currency currency, decimal amount, Guid externalId, Common.Domain.TransactionType type)
        {
            var portfolio = await GetPortfolioInternal(id);

            var cmd = new DebitCommand
            {
                Id = id,
                AssetId = portfolio.Portfolio.Single(x => x.Currency == (Currency)currency).Id,
                TransactionId = externalId,
                Amount = amount,
                Type = (TransactionType)type
            };

            await _assetClient.DebitAsync(cmd);
            await GetPortfolioInternal(id, true);
        }

        public async Task Credit(Guid id, Common.Domain.Currency currency, decimal amount, Guid externalId, Common.Domain.TransactionType type)
        {
            var portfolio = await GetPortfolioInternal(id);

            var cmd = new CreditCommand
            {
                Id = id,
                AssetId = portfolio.Portfolio.Single(x => x.Currency == (Currency)currency).Id,
                TransactionId = externalId,
                Amount = amount,
                Type = (TransactionType)type
            };

            await _assetClient.CreditAsync(cmd);
            await GetPortfolioInternal(id, true);
        }

        public async Task<Domain.Entities.Transaction[]> GetTransactions(Guid id)
        {
            var portfolio = await GetPortfolioInternal(id);
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

        private async Task<PortfolioQueryResult> GetPortfolioInternal(Guid id, bool force = false)
        {
            var portfolio = await _cacheClient.Get<PortfolioQueryResult>(id.ToString());
            if (portfolio == null || force)
            {
                portfolio = await _accountClient.PortfolioAsync(id);
                await _cacheClient.Set(id.ToString(), portfolio, _configuration.PortfolioCacheExpiration);
            }

            return portfolio;
        }
    }
}