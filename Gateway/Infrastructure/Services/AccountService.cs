using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Infrastructure.Clients.Account;
using Binebase.Exchange.Common.Infrastructure.Interfaces;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Enums;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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

        public AccountService(HttpClient client, IOptions<Configuration> options)
        {
            _configuration = options.Value;
            client.BaseAddress = _configuration.Address;
            _accountClient = new AccountClient(_configuration.Address.ToString(), client);
        }

        public async Task<decimal> GetBalance(Guid id, Common.Domain.Currency currency)
        {
            var portfolio = await _accountClient.PortfolioAsync(id);
            var result = await _accountClient.BalanceAsync(id, portfolio.Portfolio.Single(x => x.Currency == (Currency)currency).Id);
            return result.Amount;
        }

        public async Task<Dictionary<Common.Domain.Currency, decimal>> GetPorfolio(Guid id)
        {
            var result = await _accountClient.PortfolioAsync(id);
            return result.Portfolio.ToDictionary(k => (Common.Domain.Currency)k.Currency, k => k.Balance);
        }

        public async Task Create(Guid id)
            => await _accountClient.CreateAsync(new CreateAccountCommand { Id = id });

        public async Task AddCurrency(Guid id, Common.Domain.Currency currency)
            => await _accountClient.CurrencyAsync(new AddAssetCommand { Id = id, Currency = (Currency)currency });

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

            await _accountClient.DebitAsync(cmd);
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

            await _accountClient.CreditAsync(cmd);
        }

        public async Task<Domain.Entities.Transaction[]> GetTransactions(Guid id)
        {
            var portfolio = await _accountClient.PortfolioAsync(id);
            var assets = portfolio.Portfolio.ToDictionary(x => x.Id, x => new Asset { Currency = x.Currency });
            var txs = await _accountClient.TransactionsAsync(id);

            return txs.Transactions.Select(x => new Domain.Entities.Transaction
            {
                Id = x.Id,
                DateTime = x.DateTime.DateTime,
                Amount = x.Amount,
                Balance = assets[x.AssetId].Balance,
                Currency = (Common.Domain.Currency)assets[x.AssetId].Currency,
            }).ToArray();
        }

        public class Configuration
        {
            public Uri Address { get; set; }
        }
    }
}