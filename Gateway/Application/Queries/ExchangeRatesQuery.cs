using Binebase.Exchange.Gateway.Application.Interfaces;
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
                var filtered = rates.Where(x => x.Value != null);
                return new ExchangeRatesQueryResult { Rates = filtered.ToDictionary(x => x.Key.ToString(), x => x.Value.Rate) };
            }
        }
    }
}
