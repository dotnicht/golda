using Binebase.Exchange.Common.Application.Exceptions;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class SendPasswordResetCommand : IRequest
    {
        public string Email { get; set; }

        public class SendPasswordHandler : IRequestHandler<SendPasswordResetCommand>
        {
            private readonly IIdentityService _identityService;
            private readonly IEmailService _emailService;
            private readonly IAccountService _accountService;
            private readonly IDateTime _dateTime;

            public SendPasswordHandler(
                IIdentityService identityService,
                IEmailService emailService,
                IAccountService accountService,
                IDateTime dateTime)
                => (_identityService, _emailService, _accountService, _dateTime)
                    = (identityService, emailService, accountService, dateTime);

            public async Task<Unit> Handle(SendPasswordResetCommand request, CancellationToken cancellationToken)
            {
                var user = await _identityService.GetUser(request.Email);
                if (user == null) throw new NotFoundException(nameof(User), request.Email);
                if (!user.Confirmed) throw new NotSupportedException($"User with email {request.Email} not confirmed.");

                await _emailService.SendEmail(new[] { request.Email }, "Reset Password Confirmation", await _identityService.GenerateResetPasswordUrl(user.Id));

                return Unit.Value;
            }
        }
    }
}
