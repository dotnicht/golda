using Binebase.Exchange.Gateway.Application.Common.Exceptions;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Common.Domain;
using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.ValueObjects;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class ExchangeRateQuery : IRequest<ExchangeRateQueryResult>
    {
        public Currency Base { get; set; }
        public Currency Quote { get; set; }

        public class ExchangeRateQueryHandler : IRequestHandler<ExchangeRateQuery, ExchangeRateQueryResult>
        {
            private readonly IExchangeRateService _exchangeRateService;
            private readonly IDbContext _context;

            public ExchangeRateQueryHandler(IExchangeRateService exchangeRateService, IDbContext context)
                => (_exchangeRateService, _context) = (exchangeRateService, context);

            public async Task<ExchangeRateQueryResult> Handle(ExchangeRateQuery request, CancellationToken cancellationToken)
            {
                var pair = new Pair(request.Base, request.Quote);
                var rate = await _exchangeRateService.GetExchangeRate(pair);
                if (rate == null) throw new NotFoundException(nameof(ExchangeRate), pair);
                return new ExchangeRateQueryResult { Rate = rate.Rate };
            }
        }
    }
}