using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IApplicationDbContext = Binebase.Exchange.Gateway.Application.Interfaces.IApplicationDbContext;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class SignUpCommand : IRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Confirmation { get; set; }
        public string Referral { get; set; }
        public Guid? ReferenceId { get; set; }

        public class SignUpCommandHandler : IRequestHandler<SignUpCommand>
        {
            private readonly IIdentityService _identityService;
            private readonly IEmailService _emailService;
            private readonly ICalculationService _calculationService;
            private readonly IAccountService _accountService;
            private readonly ICryptoService _cryptoService;
            private readonly IDateTime _dateTime;
            private readonly IApplicationDbContext _context;
            private readonly ILogger _logger;

            public SignUpCommandHandler(
                IIdentityService identityService,
                IEmailService emailService,
                ICalculationService calculationService,
                IAccountService accountService,
                ICryptoService cryptoService,
                IDateTime dateTime,
                IApplicationDbContext context,
                ILogger<SignUpCommandHandler> logger)
                => (_identityService, _emailService, _calculationService, _accountService, _cryptoService, _dateTime, _context, _logger)
                    = (identityService, emailService, calculationService, accountService, cryptoService, dateTime, context, logger);

            public async Task<Unit> Handle(SignUpCommand request, CancellationToken cancellationToken)
            {
                // TODO: handle referral.

                var (result, userId) = await _identityService.CreateUser(request.Email, request.Password);
                if (!result.Succeeded)
                {
                    throw result.ToValidationException(nameof(SignUpCommandHandler));
                }

                var ticket = await _identityService.GenerateConfirmationToken(userId);
                await _emailService.SendEmail(new[] { request.Email }, "Email Confirmation", await _identityService.GenerateConfirmationUrl(userId));

                await _accountService.Create(userId);

                await _accountService.AddCurrency(userId, Currency.BINE);
                await _accountService.AddCurrency(userId, Currency.EURB);
                await _accountService.AddCurrency(userId, Currency.BTC);
                await _accountService.AddCurrency(userId, Currency.ETH);

                await _cryptoService.GenerateAddress(userId, Currency.BTC);
                await _cryptoService.GenerateAddress(userId, Currency.ETH);

                if (request.ReferenceId != null)
                {
                    var mining = _context.MiningRequests.SingleOrDefault(x => x.Id == request.ReferenceId.Value);
                    if (mining != null && mining.Created + await _calculationService.GetMiningRequestWindow() <= _dateTime.UtcNow)
                    {
                        await _accountService.Debit(userId, Currency.BINE, mining.Amount, TransactionSource.Mining, TransactionType.Default);
                    }
                }

                return Unit.Value;
            }
        }
    }
}
