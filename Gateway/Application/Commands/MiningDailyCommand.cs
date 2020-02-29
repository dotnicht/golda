using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Domain.Enums;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class MiningDailyCommand : IRequest<MiningDailyCommandResult>
    {
        public class MiningDailyCommandHandler : IRequestHandler<MiningDailyCommand, MiningDailyCommandResult>
        {
            private readonly ICurrentUserService _currentUserService;
            private readonly ICalculationService _calculationService;
            private readonly IAccountService _accountService;

            public MiningDailyCommandHandler(
                ICurrentUserService currentUserService,
                ICalculationService calculationService,
                IAccountService accountService)
                => (_currentUserService, _calculationService, _accountService)
                    = (currentUserService, calculationService, accountService);

            public async Task<MiningDailyCommandResult> Handle(MiningDailyCommand request, CancellationToken cancellationToken)
            {
                var timeout = await _calculationService.GetBonusTimeout();
                if (timeout > default(TimeSpan)) throw new NotSupportedException();

                var (amount, type) = await _calculationService.GenerateSimpleMiningReward();
                var result = new MiningDailyCommandResult
                {
                    Amount = amount,
                    Type = type
                };

                await _accountService.Debit(_currentUserService.UserId, Currency.BINE, result.Amount, TransactionSource.Mining, result.Type);

                return result;
            }
        }
    }
}
