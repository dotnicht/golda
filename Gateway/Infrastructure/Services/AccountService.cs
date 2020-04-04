using Binebase.Exchange.Common.Application.Exceptions;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Infrastructure.Clients.Account;
using Binebase.Exchange.Common.Infrastructure.Interfaces;
using Binebase.Exchange.Gateway.Application;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Infrastructure.Services
{
    public class AccountService : IAccountService, IConfigurationProvider<AccountService.Configuration>, IHttpClientScoped<IAccountService>
    {
        private readonly Configuration _configuration;
        private readonly AccountClient _accountClient;
        private readonly AssetClient _assetClient;
        private readonly ICacheClient _cacheClient;

        public AccountService(HttpClient client, IOptions<Configuration> options, ICacheClient cacheClient)
            => (_configuration, client.BaseAddress, _accountClient, _assetClient, _cacheClient) 
                = (options.Value, _configuration.Address, new AccountClient(_configuration.Address.ToString(), client), new AssetClient(_configuration.Address.ToString(), client), cacheClient);

        public async Task CretateDefaultAccount(Guid id)
        {
            await _accountClient.NewAsync(new NewAccountCommand { Id = id });

            foreach (var currency in _configuration.Currencies)
            {
                await AddCurrency(id, currency);
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

            throw new NotFoundException(ErrorCode.CurrencyNotSupported);
        }

        public async Task<Dictionary<Common.Domain.Currency, decimal>> GetPorfolio(Guid id)
        {
            var portfolio = await GetPortfolioInternal(id);
            return portfolio.Portfolio.ToDictionary(k => (Common.Domain.Currency)k.Currency, k => k.Balance);
        }

        public async Task Create(Guid id)
            => await _accountClient.NewAsync(new NewAccountCommand { Id = id });

        public async Task AddCurrency(Guid id, Common.Domain.Currency currency)
            => await _assetClient.AssetAsync(new AddAssetCommand { Id = id, AssetId = Guid.NewGuid(), Currency = (Currency)currency });

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
                assets[tx.AssetId].Balance += tx.Amount;
                var item = new Domain.Entities.Transaction
                {
                    Id = tx.TransactionId,
                    DateTime = tx.DateTime.DateTime,
                    Amount = tx.Amount,
                    Balance = assets[tx.AssetId].Balance,
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
                portfolio = await _assetClient.PortfolioAsync(id);
                await _cacheClient.Set(id.ToString(), portfolio, _configuration.PortfolioCacheExpiration);
            }

            return portfolio;
        }

        public class Configuration
        {
            public Uri Address { get; set; }
            public Common.Domain.Currency[] Currencies { get; set; }
            public TimeSpan PortfolioCacheExpiration { get; set; }
        }
    }
}