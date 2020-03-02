using Binebase.Exchange.Gateway.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Security;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class MultySignInCommand : IRequest<SignInCommandResult>
    {
        public Guid Id { get; set; }
        public string Code { get; set; }

        public class MultySignInCommandHandler : IRequestHandler<MultySignInCommand, SignInCommandResult>
        {
            private readonly IIdentityService _identityService;

            public MultySignInCommandHandler(IIdentityService identityService) => _identityService = identityService;

            public async Task<SignInCommandResult> Handle(MultySignInCommand request, CancellationToken cancellationToken)
            {
                var user = await _identityService.GetUser(request.Id);
                var isTfaEnabled = _identityService.GetTwoFactorEnabled(user.Id);

                if (!isTfaEnabled.Result)
                    throw new NotSupportedException();

                var iStfaValid = _identityService.VerifyTwoFactorToken(user.Id, request.Code);
                if (!iStfaValid.Result)
                    throw new SecurityException();

                var token = await _identityService.GenerateAuthToken(user);
                return new SignInCommandResult { Id = user.Id, Email = user.Email, Token = token };
            }
        }
    }
}
