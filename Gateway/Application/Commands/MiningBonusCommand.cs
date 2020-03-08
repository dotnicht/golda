using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Enums;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class MiningBonusCommand : IRequest<MiningBonusCommandResult>
    {
        public class MiningBonusCommandHandler : IRequestHandler<MiningBonusCommand, MiningBonusCommandResult>
        {
            private readonly ICurrentUserService _currentUserService;
            private readonly ICalculationService _calculationService;
            private readonly IAccountService _accountService;

            public MiningBonusCommandHandler(
                ICurrentUserService currentUserService,
                ICalculationService calculationService,
                IAccountService accountService,
                IApplicationDbContext context)
                => (_currentUserService, _calculationService, _accountService)
                    = (currentUserService, calculationService, accountService);

            public async Task<MiningBonusCommandResult> Handle(MiningBonusCommand request, CancellationToken cancellationToken)
            {
                var (amount, type) = await _calculationService.GenerateWeeklyMiningReward();

                var result = new MiningBonusCommandResult
                {
                    Amount = amount,
                    Type = type
                };

                await _accountService.Debit(_currentUserService.UserId, Currency.BINE, result.Amount, TransactionSource.Mining, result.Type);
                (amount, type) = await _calculationService.GenerateBonusMiningReward();

                if (type != TransactionType.Default && amount > 0)
                {
                    result.Type = type;
                    result.Amount += amount;
                    await _accountService.Debit(_currentUserService.UserId, Currency.BINE, amount, TransactionSource.Mining, type);
                }

                return result;
            }
        }
    }
}
