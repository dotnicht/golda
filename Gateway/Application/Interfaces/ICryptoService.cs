using Binebase.Exchange.Gateway.Common.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface ICryptoService
    {
        Task<string> GetAddress(Guid id, Currency currency);
        Task<Dictionary<Currency, string>> GetAddresses(Guid id);
        Task<string> GenerateAddress(Guid id, Currency currency);
    }
}
