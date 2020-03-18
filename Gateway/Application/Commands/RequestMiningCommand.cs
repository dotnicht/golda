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
            private readonly IApplicationDbContext _context;
            private readonly IMapper _mapper;

            public RequestMiningCommandHandler(ICalculationService calculationService, IApplicationDbContext context, IMapper mapper)
                => (_calculationService, _context, _mapper) = (calculationService, context, mapper);

            public async Task<RequestMiningCommandResult> Handle(RequestMiningCommand request, CancellationToken cancellationToken)
            {
                var item = new MiningRequest { Id = Guid.NewGuid(), Amount = await _calculationService.GenerateDefaultReward(), IsAnonymous = true };
                _context.MiningRequests.Add(item);
                await _context.SaveChangesAsync();
                return _mapper.Map<RequestMiningCommandResult>(item);
            }
        }
    }
}
