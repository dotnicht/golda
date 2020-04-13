using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Gateway.Application.Configuration;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class MiningStatusQuery : IRequest<MiningStatusQueryResult>
    {
        public class MiningStatusQueryHandler : IRequestHandler<MiningStatusQuery, MiningStatusQueryResult>
        {
            private readonly ICurrentUserService _currentUserService;
            private readonly IApplicationDbContext _context;
            private readonly IDateTime _dateTime;
            private readonly MiningCalculation _configuration;

            public MiningStatusQueryHandler(ICurrentUserService currentUserService, IApplicationDbContext context, IDateTime dateTime, IOptions<MiningCalculation> options)
                => (_currentUserService, _context, _dateTime, _configuration) = (currentUserService, context, dateTime, options.Value);

            public async Task<MiningStatusQueryResult> Handle(MiningStatusQuery request, CancellationToken cancellationToken)
            {
                var minings = _context.MiningRequests.Where(x => x.CreatedBy == _currentUserService.UserId).OrderByDescending(x => x.Created);

                return new MiningStatusQueryResult
                {
                    BonusTimeout = _configuration.Weekly.Timeout - (_dateTime.UtcNow - (await minings.FirstOrDefaultAsync(x => x.Type == MiningType.Weekly || x.Type == MiningType.Bonus || x.Type == MiningType.Default))?.Created) 
                        ?? default,
                    InstantTimeout = _configuration.Instant.Timeout - (_dateTime.UtcNow - (await minings.FirstOrDefaultAsync(x => x.Type == MiningType.Instant))?.Created) 
                        ?? default,
                    InstantMiningCount = await minings.CountAsync(x => x.Type == MiningType.Instant),
                    InstantBoostMapping = _configuration.Instant.BoostMapping
                };
            }
        }
    }
}
