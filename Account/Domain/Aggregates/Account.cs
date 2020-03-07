using Binebase.Exchange.AccountService.Domain.Events;
using Binebase.Exchange.AccountService.Domain.Exceptions;
using Binebase.Exchange.Common.Domain;
using NEventStore.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Binebase.Exchange.AccountService.Domain.Aggregates
{
    public class Account : AggregateBase
    {
        public bool Created { get; private set; }
        public IEnumerable<(Currency Currency, decimal Amount)> Portfolio => PortfolioInternal.Select(x => (x.Key, x.Value));

        private Dictionary<Currency, decimal> PortfolioInternal { get; } = new Dictionary<Currency, decimal>();

        public Account(Guid id) : this()
        {
            if (id == default)
            {
                throw new ArgumentException("Default GUID is not allowed.", nameof(id));
            }

            Id = id;
            RaiseEvent(new AccountCreatedEvent { Id = id, DateTime = DateTime.UtcNow });
        }

        private Account() : base()
        {
            Register<AccountCreatedEvent>(Apply);
            Register<AccountCurrencyAddedEvent>(Apply);
            Register<AccountCurrencyRemovedEvent>(Apply);
            Register<AccountCreditedEvent>(Apply);
            Register<AccountDebitedEvent>(Apply);
        }

        public void AddCurrency(Currency currency)
        {
            EnsureCreated();

            if (PortfolioInternal.ContainsKey(currency))
            {
                throw new AccountException($"Currency {currency} is already added to account {Id}.");
            }

            RaiseEvent(new AccountCurrencyAddedEvent { Currency = currency, DateTime = DateTime.UtcNow });
        }

        public void RemoveCurrency(Currency currency)
        {
            EnsureCreated();
            EnsureCurrencyExists(currency);

            if (PortfolioInternal[currency] != 0)
            {
                throw new AccountException($"Unable to remove currency {currency} with non-zero balance from account {Id}.");
            }

            RaiseEvent(new AccountCurrencyRemovedEvent { Currency = currency, DateTime = DateTime.UtcNow });
        }

        public Guid Debit(Currency currency, decimal amount, string payload = null)
        {
            EnsureCreated();
            EnsureCurrencyExists(currency);

            if (PortfolioInternal[currency] + amount < 0)
            {
                throw new AccountException($"Insufficient balance to perform debit operation in {currency} currency on account {Id}.");
            }

            var debit = new AccountDebitedEvent { Id = Guid.NewGuid(), Currency = currency, Amount = amount, DateTime = DateTime.UtcNow, Payload = payload };
            RaiseEvent(debit);
            return debit.Id;
        }

        public Guid Credit(Currency currency, decimal amount, string payload = null)
        {
            EnsureCreated();
            EnsureCurrencyExists(currency);

            if (PortfolioInternal[currency] - amount < 0)
            {
                throw new AccountException($"Insufficient balance to perform credit operation in {currency} currency on account {Id}.");
            }

            var credit = new AccountCreditedEvent { Id = Guid.NewGuid(), Currency = currency, Amount = amount, DateTime = DateTime.UtcNow, Payload = payload };
            RaiseEvent(credit);
            return credit.Id;
        }

        public decimal Balance(Currency currency)
        {
            EnsureCreated();
            EnsureCurrencyExists(currency);
            return PortfolioInternal[currency];
        }

        private void EnsureCreated()
        {
            if (!Created)
            {
                throw new AccountException($"Account {Id} is not created.");
            }
        }

        private void EnsureCurrencyExists(Currency currency)
        {
            if (!PortfolioInternal.ContainsKey(currency))
            {
                throw new AccountException($"Currency {currency} doesn't exist in account {Id}.");
            }
        }

        private void Apply(AccountCreatedEvent obj) => Created = true;

        private void Apply(AccountCurrencyAddedEvent obj) => PortfolioInternal.Add(obj.Currency, default);

        private void Apply(AccountCurrencyRemovedEvent obj) => PortfolioInternal.Remove(obj.Currency);

        private void Apply(AccountCreditedEvent obj) => PortfolioInternal[obj.Currency] -= obj.Amount;

        private void Apply(AccountDebitedEvent obj) => PortfolioInternal[obj.Currency] += obj.Amount;
    }
}
