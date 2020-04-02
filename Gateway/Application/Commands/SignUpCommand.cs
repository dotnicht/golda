using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class SignUpCommand : IRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ReferralCode { get; set; }
        public Guid? MiningRequestId { get; set; }

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
                var id = request.MiningRequestId ?? Guid.NewGuid();
                var result = await _identityService.CreateUser(id, request.Email, request.Password, request.ReferralCode);
                if (!result.Succeeded)
                {
                    throw result.ToValidationException(nameof(SignUpCommandHandler));
                }

                var ticket = await _identityService.GenerateConfirmationToken(id);
                await _emailService.SendEmail(new[] { request.Email }, "Email Confirmation", await _identityService.GenerateConfirmationUrl(id));
                
                await _accountService.Create(id);
                await _accountService.AddCurrency(id, Currency.BINE);
                await _accountService.AddCurrency(id, Currency.EURB);
                await _accountService.AddCurrency(id, Currency.BTC);
                await _accountService.AddCurrency(id, Currency.ETH);

                await _cryptoService.GenerateAddress(id, Currency.BTC);
                await _cryptoService.GenerateAddress(id, Currency.ETH);

                var mining = _context.MiningRequests.SingleOrDefault(x => x.Id == request.MiningRequestId);
                if (mining != null && mining.Created + _calculationService.MiningRequestWindow <= _dateTime.UtcNow && mining.IsAnonymous)
                {
                    await _accountService.Debit(id, Currency.BINE, mining.Amount, mining.Id, TransactionType.Mining);
                    mining.LastModifiedBy = id;
                    mining.IsAnonymous = false;
                    await _context.SaveChangesAsync();
                }

                await _accountService.Debit(id, Currency.EURB, 100, Guid.NewGuid(), TransactionType.SignUp);
                
                return Unit.Value;
            }
        }
    }
}
