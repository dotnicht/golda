using Binebase.Exchange.Gateway.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class MiningStatusQuery : IRequest<MiningStatusQueryResult>
    {
        public class MiningStatusQueryHandler : IRequestHandler<MiningStatusQuery, MiningStatusQueryResult>
        {
            private readonly ICalculationService _calculationService;

            public MiningStatusQueryHandler(ICalculationService calculationService)
                => _calculationService = calculationService;

            public async Task<MiningStatusQueryResult> Handle(MiningStatusQuery request, CancellationToken cancellationToken)
                => new MiningStatusQueryResult
                    {
                        WeeklyTimeout = await _calculationService.GetWeeklyTimeout(),
                        InstantTimeout = await _calculationService.GetInstantTimeout(),
                        CurrentMiningCount = await _calculationService.GetCurrentMiningCount(),
                        BoostMapping = await _calculationService.GetInstantBoostMapping()
                    };

        }
    }
}
