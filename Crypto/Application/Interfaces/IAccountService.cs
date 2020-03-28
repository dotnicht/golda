using Binebase.Exchange.Common.Domain;
using System;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Application.Interfaces
{
    public interface IAccountService
    {
        Task<Guid> Debit(Guid accountId, Currency currency, decimal amount, Guid externalId);
        Task<Guid> Credit(Guid accountId, Currency currency, decimal amount, Guid externalId);
    }
}
