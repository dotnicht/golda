using AutoMapper;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class RequestMiningCommand : IRequest<RequestMiningCommandResult>
    {
        public class RequestMiningCommandHandler : IRequestHandler<RequestMiningCommand, RequestMiningCommandResult>
        {
            private readonly ICalculationService _calculationService;
            private readonly ICurrentUserService _currentUserService;
            private readonly IApplicationDbContext _context;
            private readonly IMapper _mapper;

            public RequestMiningCommandHandler(ICalculationService calculationService, ICurrentUserService currentUserService, IApplicationDbContext context, IMapper mapper)
                => (_calculationService, _currentUserService, _context, _mapper) = (calculationService, currentUserService, context, mapper);

            public async Task<RequestMiningCommandResult> Handle(RequestMiningCommand request, CancellationToken cancellationToken)
            {
                if (!_currentUserService.IsAnonymous)
                {
                    throw new NotSupportedException(ErrorCode.MiningRequestNotSupported);
                }

                var amount = await _calculationService.GenerateDefaultReward();
                var id = Guid.NewGuid();
                var item = new MiningRequest { Id = id, CreatedBy = id, Amount = amount, IsAnonymous = true };
                _context.MiningRequests.Add(item);
                await _context.SaveChangesAsync();
                return _mapper.Map<RequestMiningCommandResult>(item);
            }
        }
    }
}
