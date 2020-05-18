using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.ValueObjects;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class ExchangeRatesQuery : IRequest<ExchangeRatesQueryResult>
    {
        public class ExchangeRatesQueryHandler : IRequestHandler<ExchangeRatesQuery, ExchangeRatesQueryResult>
        {
            private readonly IExchangeRateService _exchangeRateService;

            public ExchangeRatesQueryHandler(IExchangeRateService exchangeRateService) => _exchangeRateService = exchangeRateService;

            public async Task<ExchangeRatesQueryResult> Handle(ExchangeRatesQuery request, CancellationToken cancellationToken)
            {
                var rates = await _exchangeRateService.GetExchangeRates();
                return new ExchangeRatesQueryResult { Rates = rates.ToDictionary(x => new Pair(x.Base, x.Quote).ToString(), x => x.Rate) };
            }
        }
    }
}
