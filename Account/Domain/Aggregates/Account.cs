using Binebase.Exchange.AccountService.Domain.Entities;
using Binebase.Exchange.AccountService.Domain.Events;
using Binebase.Exchange.AccountService.Domain.Exceptions;
using Binebase.Exchange.Common.Domain;
using NEventStore.Domain.Core;
using System;
using System.Collections.Generic;

namespace Binebase.Exchange.AccountService.Domain.Aggregates
{
    public class Account : AggregateBase
    {
        public bool Exists { get; private set; }
        public bool IsLocked { get; private set; }

        public IEnumerable<Asset> Portfolio  
        { 
            get 
            { 
                EnsureExists();
                return PortfolioInternal.Values;
            } 
        }

        private Dictionary<Guid, Asset> PortfolioInternal { get; } = new Dictionary<Guid, Asset>();

        public Account(Guid id, DateTime dateTime) : this()
        {
            if (id == default)
            {
                throw new ArgumentException("Default GUID is not allowed.", nameof(id));
            }

            if (dateTime == default)
            {
                throw new ArgumentException("Default DateTime is not allowed.", nameof(dateTime));
            }

            Id = id;
            RaiseEvent(new AccountCreatedEvent { Id = id, DateTime = dateTime });
        }

        private Account() : base()
        {
            Register<AccountCreatedEvent>(Apply);
            Register<AssetAddedEvent>(Apply);
            Register<AssetRemovedEvent>(Apply);
            Register<CreditedEvent>(Apply);
            Register<DebitedEvent>(Apply);
            Register<AccountLockedEvent>(Apply);
            Register<AccountUnlockedEvent>(Apply);
            Register<AssetLockedEvent>(Apply);
            Register<AssetUnlockedEvent>(Apply);
        }

        public void AddAsset(Guid id, Currency currency, DateTime dateTime)
        {
            EnsureExists();
            EnsureUnlocked();
            EnsureUnlocked();

            if (PortfolioInternal.ContainsKey(id))
            {
                throw new AccountException(ErrorCode.AssetExists);
            }

            RaiseEvent(new AssetAddedEvent { Currency = currency, DateTime = DateTime.UtcNow });
        }

        public void RemoveAsset(Guid id, DateTime dateTime)
        {
            EnsureExists();
            EnsureUnlocked();
            EnsureAssetExists(id);

            if (PortfolioInternal[id].Balance != 0)
            {
                throw new AccountException(ErrorCode.NonZeroBalance);
            }

            RaiseEvent(new AssetRemovedEvent { Id = id, DateTime = dateTime });
        }

        public void Debit(Guid id, Guid txId, decimal amount, DateTime dateTime, TransactionType type)
        {
            EnsureExists();
            EnsureUnlocked();
            EnsureAssetExists(id);
            EnsureAssetUnlocked(id);

            if (PortfolioInternal[id].Balance + amount < 0)
            {
                throw new AccountException(ErrorCode.InsufficientBalance);
            }

            var debit = new DebitedEvent { Id = txId, Amount = amount, DateTime = dateTime, Type = type };
            RaiseEvent(debit);
        }

        public void Credit(Guid id, Guid txId, decimal amount, DateTime dateTime, TransactionType type)
        {
            EnsureExists();
            EnsureUnlocked();
            EnsureAssetExists(id);
            EnsureAssetUnlocked(id);

            if (PortfolioInternal[id].Balance - amount < 0)
            {
                throw new AccountException(ErrorCode.InsufficientBalance);
            }

            var credit = new CreditedEvent { Id = txId, Amount = amount, DateTime = dateTime, Type = type};
            RaiseEvent(credit);
        }

        public void Lock(DateTime dateTime)
        {
            EnsureExists();
            EnsureUnlocked();
            RaiseEvent(new AccountLockedEvent { DateTime = dateTime });
        }

        public void Unlock(DateTime dateTime)
        {
            EnsureExists();

            if (!IsLocked)
            {
                throw new AccountException(ErrorCode.AccountUnlocked);
            }

            RaiseEvent(new AccountUnlockedEvent { DateTime = dateTime });
        }

        public void Lock(Guid id, DateTime dateTime)
        {
            EnsureExists();
            EnsureUnlocked();
            EnsureAssetExists(id);
            EnsureAssetUnlocked(id);

            RaiseEvent(new AssetLockedEvent { Id = id, DateTime = dateTime });
        }

        public void Unlock(Guid id, DateTime dateTime)
        {
            EnsureExists();
            EnsureUnlocked();
            EnsureAssetExists(id);

            if (!PortfolioInternal[id].IsLocked)
            {
                throw new AccountException(ErrorCode.AssetUnlocked);
            }

            RaiseEvent(new AssetUnlockedEvent { Id = id, DateTime = dateTime });
        }

        public decimal Balance(Guid id)
        {
            EnsureExists();
            EnsureAssetExists(id);
            return PortfolioInternal[id].Balance;
        }

        private void EnsureExists()
        {
            if (!Exists)
            {
                throw new AccountException(ErrorCode.AccountNotExists);
            }
        }

        private void EnsureUnlocked()
        {
            if (IsLocked)
            {
                throw new AccountException(ErrorCode.AccountLocked);
            }
        }

        private void EnsureAssetExists(Guid id)
        {
            if (!PortfolioInternal.ContainsKey(id))
            {
                throw new AccountException(ErrorCode.AssetNotExists);
            }
        }

        private void EnsureAssetUnlocked(Guid id)
        {
            if (PortfolioInternal[id].IsLocked)
            {
                throw new AccountException(ErrorCode.AssetLocked);
            }
        }

        private void Apply(AccountCreatedEvent obj) => Exists = true;
        private void Apply(AssetAddedEvent obj) => PortfolioInternal.Add(obj.Id, new Asset { Id = obj.Id, Currency = obj.Currency });
        private void Apply(AssetRemovedEvent obj) => PortfolioInternal.Remove(obj.Id);
        private void Apply(CreditedEvent obj) => PortfolioInternal[obj.Id].Balance -= obj.Amount;
        private void Apply(DebitedEvent obj) => PortfolioInternal[obj.Id].Balance += obj.Amount;
        private void Apply(AccountLockedEvent obj) => IsLocked = true;
        private void Apply(AccountUnlockedEvent obj) => IsLocked = false;
        private void Apply(AssetLockedEvent obj) => PortfolioInternal[obj.Id].IsLocked = true;
        private void Apply(AssetUnlockedEvent obj) => PortfolioInternal[obj.Id].IsLocked = false;
    }
}
