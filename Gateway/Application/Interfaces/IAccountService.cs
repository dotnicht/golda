﻿using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface IAccountService
    {
        Task CretateDefaultAccount(Guid id);
        Task<decimal> GetBalance(Guid id, Currency currency);
        Task<Dictionary<Currency, decimal>> GetPorfolio(Guid id, bool force = false);
        Task Debit(Guid id, Currency currency, decimal amount, Guid externalId, TransactionType source);
        Task Credit(Guid id, Currency currency, decimal amount, Guid externalId, TransactionType source);
        Task<Transaction[]> GetTransactions(Guid id);
    }
}
