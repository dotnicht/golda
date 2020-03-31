using AutoMapper;
using Binebase.Exchange.Gateway.Application.Interfaces;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class MiningsQuery : IRequest<MiningsQueryResult>
    {
        public class MiningsQueryHandler : IRequestHandler<MiningsQuery, MiningsQueryResult>
        {
            private readonly IApplicationDbContext _context;
            private readonly ICurrentUserService _currentUserService;
            private readonly IMapper _mapper;

            public MiningsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IMapper mapper)
                => (_context, _currentUserService, _mapper) = (context, currentUserService, mapper);

            public async Task<MiningsQueryResult> Handle(MiningsQuery request, CancellationToken cancellationToken)
                => await Task.FromResult(new MiningsQueryResult { Minings = _mapper.Map<MiningsQueryResult.Mining[]>(_context.MiningRequests.Where(x => x.CreatedBy == _currentUserService.UserId)) });
        }
    }
}
