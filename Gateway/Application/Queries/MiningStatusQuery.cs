using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Enums;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class MiningStatusQuery : IRequest<MiningStatusQueryResult>
    {
        public class MiningStatusQueryHandler : IRequestHandler<MiningStatusQuery, MiningStatusQueryResult>
        {
            private readonly ICalculationService _calculationService;
            private readonly ICurrentUserService _currentUserService;
            private readonly IApplicationDbContext _context;
            private readonly IDateTime _dateTime;

            public MiningStatusQueryHandler(ICalculationService calculationService, ICurrentUserService currentUserService, IApplicationDbContext context, IDateTime dateTime)
                => (_calculationService, _currentUserService, _context, _dateTime) = (calculationService, currentUserService, context, dateTime);

            public async Task<MiningStatusQueryResult> Handle(MiningStatusQuery request, CancellationToken cancellationToken)
            {
                var minings = _context.MiningRequests.Where(x => x.CreatedBy == _currentUserService.UserId).OrderByDescending(x => x.Created);

                return new MiningStatusQueryResult
                {
                    BonusTimeout = _dateTime.UtcNow - minings.FirstOrDefault(x => x.Type == TransactionType.Bonus)?.Created ?? default,
                    InstantTimeout = _dateTime.UtcNow - minings.FirstOrDefault(x => x.Type == TransactionType.Instant)?.Created ?? default,
                    CurrentMiningCount = await _calculationService.GetCurrentMiningCount(),
                    BoostMapping = _calculationService.InstantBoostMapping
                };
            }
        }
    }
}
