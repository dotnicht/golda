using AutoMapper;
using Binebase.Exchange.Common.Application.Exceptions;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class MiningIndexQuery : IRequest<MiningIndexQueryResult>
    {
        public int Index { get; set; }

        public class MiningIndexQueryRequestHandler : IRequestHandler<MiningIndexQuery, MiningIndexQueryResult>
        {
            private readonly IApplicationDbContext _context;
            private readonly IMapper _mapper;

            public MiningIndexQueryRequestHandler(IApplicationDbContext context, IMapper mapper) 
                => (_context, _mapper) = (context, mapper);

            public async Task<MiningIndexQueryResult> Handle(MiningIndexQuery request, CancellationToken cancellationToken)
                => _mapper.Map<MiningIndexQueryResult>(await _context.MiningRequests.SingleOrDefaultAsync(x => x.Index == request.Index))
                    ?? throw new NotFoundException(nameof(MiningRequest), request.Index);
        }
    }
}
