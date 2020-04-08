using Binebase.Exchange.Common.Application.Exceptions;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Enums;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class WithdrawCommand : IRequest<WithdrawCommandResult>
    {
        public Currency Currency { get; set; }
        public decimal Amount { get; set; }
        public string Address { get; set; }
        public string Code { get; set; }

        public class WithdrawCommandHandler : IRequestHandler<WithdrawCommand, WithdrawCommandResult>
        {
            private readonly IApplicationDbContext _context;
            private readonly ICryptoService _cryptoService;
            private readonly IAccountService _accountService;
            private readonly IIdentityService _identityService;
            private readonly ICalculationService _calculationService;
            private readonly ICurrentUserService _currentUserService;

            public WithdrawCommandHandler(
                IApplicationDbContext context,
                ICryptoService cryptoService,
                IAccountService accountService,
                IIdentityService identityService,
                ICalculationService calculationService,
                ICurrentUserService currentUserService)
                => (_context, _cryptoService, _accountService, _identityService, _calculationService, _currentUserService) 
                    = (context, cryptoService, accountService, identityService, calculationService, currentUserService);

            public async Task<WithdrawCommandResult> Handle(WithdrawCommand request, CancellationToken cancellationToken)
            {
                /*
                if (_context.MiningRequests.Count(x => x.CreatedBy == _currentUserService.UserId && x.Type == MiningType.Instant) < _calculationService.OperationLockMiningCount)
                {
                    throw new NotSupportedException(ErrorCode.InsufficientMinings);
                }

                if (!await _identityService.GetTwoFactorEnabled(_currentUserService.UserId))
                {
                    throw new NotSupportedException(ErrorCode.MultiFactorRequired);
                }

                if (!await _identityService.VerifyTwoFactorToken(_currentUserService.UserId, request.Code))
                {
                    throw new SecurityException(ErrorCode.MultiFactor);
                }
                */
                var id = Guid.NewGuid();
                await _accountService.Credit(_currentUserService.UserId, request.Currency, request.Amount, id, TransactionType.Widthraw);
                return new WithdrawCommandResult { Hash = await _cryptoService.PublishTransaction(_currentUserService.UserId, request.Currency, request.Amount, request.Address, id) };
            }
        }
    }
}
