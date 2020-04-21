using Binebase.Exchange.Common.Application.Exceptions;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class RequestResetPasswordCommand : IRequest
    {
        public string Email { get; set; }

        public class RequestResetPasswordHandler : IRequestHandler<RequestResetPasswordCommand>
        {
            private readonly IIdentityService _identityService;
            private readonly IEmailService _emailService;
            private readonly IAccountService _accountService;
            private readonly IDateTime _dateTime;

            public RequestResetPasswordHandler(
                IIdentityService identityService,
                IEmailService emailService,
                IAccountService accountService,
                IDateTime dateTime)
                => (_identityService, _emailService, _accountService, _dateTime)
                    = (identityService, emailService, accountService, dateTime);

            public async Task<Unit> Handle(RequestResetPasswordCommand request, CancellationToken cancellationToken)
            {
                var user = await _identityService.GetUser(request.Email);

                if (user == null)
                {
                    throw new NotFoundException(nameof(User), request.Email);
                }

                if (!user.Confirmed)
                {
                    throw new NotSupportedException(ErrorCode.ConfirmationRequired);
                }

                await _emailService.SendEmail(new[] { request.Email }, "Reset Password Confirmation", await _identityService.GenerateResetPasswordUrl(user.Id), EmailType.ResetPassword);

                return Unit.Value;
            }
        }
    }
}
