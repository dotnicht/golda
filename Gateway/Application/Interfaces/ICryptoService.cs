using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface ICryptoService
    {
        Task GenerateDefaultAddresses(Guid id);
        Task<string> GetAddress(Guid id, Currency currency);
        Task<Dictionary<Currency, string>> GetAddresses(Guid id);
        Task<string> GenerateAddress(Guid id, Currency currency);
        Task<Transaction[]> GetTransactions(Guid id);
        Task<string> PublishTransaction(Guid id, Currency currency, decimal amount, string address, Guid externalId);
        Task Transfer();
    }
}
