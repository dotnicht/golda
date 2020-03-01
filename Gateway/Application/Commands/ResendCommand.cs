using Binebase.Exchange.Gateway.Application.Common.Exceptions;
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
    public class ResendCommand : IRequest
    {
        public string Email { get; set; }

        public class ResendCommandHandler : IRequestHandler<ResendCommand>, IConfigurationProvider<ResendCommandHandler.Configuration>
        {
            private readonly IIdentityService _identityService;
            private readonly IEmailService _emailService;
            private readonly IAccountService _accountService;
            private readonly IDateTime _dateTime;
            private readonly IOptions<Configuration> _options;

            public ResendCommandHandler(
                IIdentityService identityService,
                IEmailService emailService,
                IAccountService accountService,
                IDateTime dateTime,
                IOptions<Configuration> options)
                => (_identityService, _emailService, _accountService, _dateTime, _options)
                    = (identityService, emailService, accountService,  dateTime, options);

            public async Task<Unit> Handle(ResendCommand request, CancellationToken cancellationToken)
            {
                var user = await _identityService.GetUser(request.Email);
                if (user == null)
                    throw new NotFoundException(nameof(User), request.Email);
                if (user.Confirmed)
                    throw new NotSupportedException($"User with email: {request.Email} already confirmed");
                await _emailService.SendEmail(new[] { request.Email }, "Email Confirmation", await _identityService.GenerateConfirmationUrl(user.Id));

                return Unit.Value;
            }

            public class Configuration
            {
                public string SomeValue { get; set; }
            }
        }
    }
}
