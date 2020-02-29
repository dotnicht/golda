using AutoMapper;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class MiningRequestCommand : IRequest<MiningRequestCommandResult>
    {
        public class MiningRequestCommandHandler : IRequestHandler<MiningRequestCommand, MiningRequestCommandResult>
        {
            private readonly ICalculationService _calculationService;
            private readonly IDbContext _context;
            private readonly IMapper _mapper;

            public MiningRequestCommandHandler(ICalculationService calculationService, IDbContext context, IMapper mapper)
                => (_calculationService, _context, _mapper) = (calculationService, context, mapper);

            public async Task<MiningRequestCommandResult> Handle(MiningRequestCommand request, CancellationToken cancellationToken)
            {
                var item = new MiningRequest { Id = Guid.NewGuid(), Amount = await _calculationService.GenerateDefaultReward() };
                _context.MiningRequests.Add(item);
                await _context.SaveChangesAsync();
                return _mapper.Map<MiningRequestCommandResult>(item);
            }
        }
    }
}
