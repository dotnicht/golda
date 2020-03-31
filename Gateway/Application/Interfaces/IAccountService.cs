using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface IAccountService
    {
        Task<decimal> GetBalance(Guid id, Currency currency);
        Task<Dictionary<Currency, decimal>> GetPorfolio(Guid id);
        Task Create(Guid id);
        Task AddCurrency(Guid id, Currency currency);
        Task Debit(Guid id, Currency currency, decimal amount, Guid externalId, TransactionType source);
        Task Credit(Guid id, Currency currency, decimal amount, Guid externalId, TransactionType source);
        Task<Transaction[]> GetTransactions(Guid id);
    }
}
