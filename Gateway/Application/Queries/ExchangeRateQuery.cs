using Binebase.Exchange.Common.Application.Exceptions;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.ValueObjects;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class ExchangeRateQuery : IRequest<ExchangeRateQueryResult>
    {
        public Currency Base { get; set; }
        public Currency Quote { get; set; }

        public class ExchangeRateQueryHandler : IRequestHandler<ExchangeRateQuery, ExchangeRateQueryResult>
        {
            private readonly IExchangeRateService _exchangeRateService;
            private readonly IApplicationDbContext _context;
            private readonly IMapper _mapper;

            public ExchangeRateQueryHandler(IExchangeRateService exchangeRateService, IApplicationDbContext context, IMapper mapper)
                => (_exchangeRateService, _context, _mapper) = (exchangeRateService, context, mapper);

            public async Task<ExchangeRateQueryResult> Handle(ExchangeRateQuery request, CancellationToken cancellationToken)
            {
                var pair = new Pair(request.Base, request.Quote);
                var rates = await _exchangeRateService.GetExchangeRateHistory(pair);
                if (rates == null)
                {
                    throw new NotFoundException(nameof(ExchangeRate), pair);
                }

                return new ExchangeRateQueryResult { Rates = _mapper.Map<ExchangeRateQueryResult.ExchangeRate[]>(rates) };
            }
        }
    }
}