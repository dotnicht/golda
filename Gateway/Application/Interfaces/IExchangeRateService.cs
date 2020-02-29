using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface IExchangeRateService
    {
        Task<ExchangeRate> GetExchangeRate(Pair pair);
        Task<Dictionary<Pair, ExchangeRate>> GetExchangeRates();
        Task Subscribe();
    }
}
