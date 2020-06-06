using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Application.Configuration;
using Binebase.Exchange.Gateway.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
            private readonly IAccountService _accountService;
            private readonly ICryptoService _cryptoService;
            private readonly IDateTime _dateTime;
            private readonly IApplicationDbContext _context;
            private readonly ILogger _logger;
            private readonly MiningCalculation _configuration;

            public SignUpCommandHandler(
                IIdentityService identityService,
                IEmailService emailService,
                IAccountService accountService,
                ICryptoService cryptoService,
                IDateTime dateTime,
                IApplicationDbContext context,
                ILogger<SignUpCommandHandler> logger,
                IOptions<MiningCalculation> options)
                => (_identityService, _emailService, _accountService, _cryptoService, _dateTime, _context, _logger, _configuration)
                    = (identityService, emailService, accountService, cryptoService, dateTime, context, logger, options.Value);

            public async Task<Unit> Handle(SignUpCommand request, CancellationToken cancellationToken)
            {
                if (request.MiningRequestId != null)
                {
                    var user = await _identityService.GetUser(request.MiningRequestId.Value);
                    if (user != null)
                    {
                        throw new NotSupportedException(ErrorCode.MiningRequestNotSupported);
                    }
                }

                var mining = _context.MiningRequests.SingleOrDefault(x => x.Id == request.MiningRequestId);

                var id = mining == null ? Guid.NewGuid() : request.MiningRequestId.Value;

                var result = await _identityService.CreateUser(id, request.Email, request.Password, request.ReferralCode);
                if (!result.Succeeded)
                {
                    throw result.ToValidationException(nameof(SignUpCommandHandler));
                }

                await _accountService.CretateDefaultAccount(id);
                await _cryptoService.GenerateDefaultAddresses(id);

                if (mining != null && mining.IsAnonymous && _dateTime.UtcNow - mining.Created <= _configuration.MiningRequestWindow)
                {
                    await _accountService.Debit(id, Currency.BINE, mining.Amount, mining.Id, TransactionType.Mining);
                    mining.IsAnonymous = false;
                    mining.LastModifiedBy = id;
                    await _context.SaveChangesAsync();
                }

                await _accountService.Debit(id, Currency.EURB, 50, Guid.NewGuid(), TransactionType.SignUp);
                await _emailService.SendConfirmRegistrationEmail(new[] { request.Email }, "Email Confirmation", await _identityService.GenerateConfirmationUrl(id));

                return Unit.Value;
            }
        }
    }
}
