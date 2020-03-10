using Binebase.Exchange.Gateway.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Security;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class SignInMultyFactorCommand : IRequest<SignInCommandResult>
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Password { get; set; }

        public class MultyFactorSignInCommandHandler : IRequestHandler<SignInMultyFactorCommand, SignInCommandResult>
        {
            private readonly IIdentityService _identityService;

            public MultyFactorSignInCommandHandler(IIdentityService identityService) => _identityService = identityService;

            public async Task<SignInCommandResult> Handle(SignInMultyFactorCommand request, CancellationToken cancellationToken)
            {
                var user = await _identityService.GetUser(request.Id);
                var isTfaEnabled = _identityService.GetTwoFactorEnabled(user.Id);            

                if (!isTfaEnabled.Result)
                    throw new NotSupportedException();

                if (!await _identityService.CheckUserPassword(user.Id, request.Password))
                     throw new SecurityException();

                var iStfaValid = await _identityService.VerifyTwoFactorToken(user.Id, request.Code);
                if (!iStfaValid)
                    throw new SecurityException();

                var token = await _identityService.GenerateAuthToken(user);
                return new SignInCommandResult { Id = user.Id, Email = user.Email, Token = token };
            }
        }
    }
}
