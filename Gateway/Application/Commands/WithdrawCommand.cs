using Binebase.Exchange.Common.Application.Exceptions;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Application.Configuration;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Enums;
using Binebase.Exchange.Gateway.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Runtime.InteropServices;
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
            private IDateTime _dateTime;
            private readonly IApplicationDbContext _context;
            private readonly ICryptoService _cryptoService;
            private readonly IAccountService _accountService;
            private readonly IIdentityService _identityService;
            private readonly ICurrentUserService _currentUserService;
            private readonly IExchangeRateService _exchangeRateService;
            private readonly CryptoOperations _configuration;

            public WithdrawCommandHandler(
                IDateTime dateTime,
                IApplicationDbContext context,
                ICryptoService cryptoService,
                IAccountService accountService,
                IIdentityService identityService,
                ICurrentUserService currentUserService,
                IExchangeRateService exchangeRateService,
                IOptions<CryptoOperations> options)
                => (_dateTime, _context, _cryptoService, _accountService, _identityService, _currentUserService, _exchangeRateService, _configuration)
                    = (dateTime, context, cryptoService, accountService, identityService, currentUserService, exchangeRateService, options.Value);

            public async Task<WithdrawCommandResult> Handle(WithdrawCommand request, CancellationToken cancellationToken)
            {
                if (!new[] { Currency.BTC, Currency.ETH }.Contains(request.Currency))
                {
                    throw new NotSupportedException(ErrorCode.CurrencyNotSupported);
                }

                if (_configuration.WithdrawMiningRequirement > 0 && _context.MiningRequests.Count(x => x.CreatedBy == _currentUserService.UserId && x.Type == MiningType.Instant) < _configuration.WithdrawMiningRequirement)
                {
                    throw new NotSupportedException(ErrorCode.InsufficientMinings);
                }

                if (_configuration.WithdrawMultiRequired)
                {
                    if (!await _identityService.GetTwoFactorEnabled(_currentUserService.UserId))
                    {
                        throw new NotSupportedException(ErrorCode.MultiFactorRequired);
                    }

                    if (!await _identityService.VerifyTwoFactorToken(_currentUserService.UserId, request.Code))
                    {
                        throw new SecurityException(ErrorCode.MultiFactor);
                    }
                }

                if (_configuration.WithdrawDailyLimit > 0)
                {
                    var eurb = 0M;
                    foreach (var tx in _context.Transactions.Where(x => x.DateTime.Date == _dateTime.UtcNow.Date && x.Type == TransactionType.Withdraw && x.CreatedBy == _currentUserService.UserId))
                    {
                        eurb += tx.Amount * (await _exchangeRateService.GetExchangeRate(new Pair(Currency.EURB, tx.Currency), false)).Rate;
                    }

                    if (_configuration.WithdrawDailyLimit <= eurb)
                    {
                        throw new NotSupportedException(ErrorCode.WithdrawLimit);
                    }
                }

                var id = Guid.NewGuid();
                await _accountService.Credit(_currentUserService.UserId, request.Currency, request.Amount, id, TransactionType.Withdraw);
                return new WithdrawCommandResult { Hash = await _cryptoService.PublishTransaction(_currentUserService.UserId, request.Currency, request.Amount, request.Address, id) };
            }
        }
    }
}
