using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface IExchangeRateProvider
    {
        Task Subscribe(Pair pair, Action<ExchangeRate> handle);
    }
}
