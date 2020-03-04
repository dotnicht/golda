using Binebase.Exchange.Gateway.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class ConfirmCommand : IRequest<ConfirmCommandResult>
    {
        public Guid Id { get; set; }
        public string Code { get; set; }

        public class ConfirmCommandHandler : IRequestHandler<ConfirmCommand, ConfirmCommandResult>
        {
            private readonly IIdentityService _identityService;
            private readonly IAccountService _accountService;

            public ConfirmCommandHandler(IIdentityService identityService, IAccountService accountService)
                => (_identityService, _accountService) = (identityService, accountService);

            public async Task<ConfirmCommandResult> Handle(ConfirmCommand request, CancellationToken cancellationToken)
            {
                var confirmResult = await _identityService.ConfirmToken(request.Id, request.Code);

                if (!confirmResult.Succeeded) 
                { 
                    throw confirmResult.ToValidationException(nameof(ConfirmCommandHandler)); 
                }

                var user = await _identityService.GetUser(request.Id);
                var authResult = await _identityService.Authenticate(user);

                if (!authResult.Succeeded)
                {
                    throw authResult.ToSecurityException();
                }

                var token = await _identityService.GenerateAuthToken(user);
                return new ConfirmCommandResult { Id = user.Id, Email = user.Email, Token = token };
            }
        }
    }
}
