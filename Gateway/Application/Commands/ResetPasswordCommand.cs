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
    public class ResetPasswordCommand : IRequest
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Password { get; set; }

        public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand>
        {
            private readonly IIdentityService _identityService;
            private readonly IEmailService _emailService;
            private readonly IAccountService _accountService;
            private readonly IDateTime _dateTime;

            public ResetPasswordHandler(
                IIdentityService identityService,
                IEmailService emailService,
                IAccountService accountService,
                IDateTime dateTime)
                => (_identityService, _emailService, _accountService, _dateTime)
                    = (identityService, emailService, accountService, dateTime);

            public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
            {
                var user = await _identityService.GetUser(request.Id);

                if (user == null)
                {
                    throw new NotFoundException(nameof(User), request.Id);
                }

                if (!user.Confirmed)
                {
                    throw new NotSupportedException(ErrorCode.ConfirmationRequired);
                }

                var result = await _identityService.ResetPassword(request.Id, request.Code, request.Password);

                if (!result.Succeeded)
                {
                    throw result.ToSecurityException();
                }

                return Unit.Value;
            }
        }
    }
}