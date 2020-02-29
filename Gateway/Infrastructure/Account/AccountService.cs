using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Common.Interfaces;
using Binebase.Exchange.Gateway.Domain.Enums;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Infrastructure.Account
{
    public class AccountService : IAccountService, IConfigurationProvider<AccountService.Configuration>, ITransient<IAccountService>
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
            var result = await _accountClient.BalanceAsync(id, (Currency)currency);
            return result.Amount;
        }

        public async Task<Dictionary<Common.Domain.Currency, decimal>> GetPorfolio(Guid id)
        {
            var result = await _accountClient.PortfolioAsync(id);
            return result.Portfolio.ToDictionary(k => (Common.Domain.Currency)k.Key, k => k.Value);
        }

        public async Task Create(Guid id)
            => await _accountClient.CreateAsync(new CreateAccountCommand { Id = id });

        public async Task AddCurrency(Guid id, Common.Domain.Currency currency)
            => await _accountClient.CurrencyAsync(new AddCurrencyCommand { Id = id, Currency = (Currency)currency });

        public async Task RemoveCurrency(Guid id, Common.Domain.Currency currency)
            => await _accountClient.Currency2Async(new RemoveCurrencyCommand { Id = id, Currency = (Currency)currency });

        public async Task<Guid> Debit(Guid id, Common.Domain.Currency currency, decimal amount, TransactionSource source, TransactionType? type = null)
        {
            var cmd = new DebitAccountCommand
            {
                Id = id,
                Currency = (Currency)currency,
                Amount = amount,
                Payload = JsonConvert.SerializeObject(new TransactionPayload { Source = source, Type = type })
            };

            return (await _accountClient.DebitAsync(cmd)).Id;
        }

        public async Task<Guid> Credit(Guid id, Common.Domain.Currency currency, decimal amount, TransactionSource source, TransactionType? type = null)
        {
            var cmd = new CreditAccountCommand
            {
                Id = id,
                Currency = (Currency)currency,
                Amount = amount,
                Payload = JsonConvert.SerializeObject(new TransactionPayload { Source = source, Type = type })
            };

            return (await _accountClient.CreditAsync(cmd)).Id;
        }

        public async Task<Domain.Entities.Transaction[]> GetTransactions(Guid id, Common.Domain.Currency currency)
        {
            var txs = await _accountClient.TransactionsAsync(id, (Currency)currency);
            var result = new List<Domain.Entities.Transaction>();

            foreach (var tx in txs.Transactions)
            {
                var payload = JsonConvert.DeserializeObject<TransactionPayload>(tx.Payload);
                var item = new Domain.Entities.Transaction
                {
                    Id = tx.Id,
                    DateTime = tx.DateTime.DateTime,
                    Amount = tx.Amount,
                    Balance = tx.Balance,
                    Currency = currency,
                    Source = payload.Source,
                    Type = payload.Type
                };

                result.Add(item);
            }

            return result.ToArray();
        }

        public class Configuration
        {
            public Uri Address { get; set; }
        }

        private class TransactionPayload
        {
            public TransactionSource Source { get; set; }
            public TransactionType? Type { get; set; }
        }
    }
}