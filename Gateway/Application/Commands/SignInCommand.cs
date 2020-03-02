using Binebase.Exchange.Gateway.Application.Interfaces;
using MediatR;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class SignInCommand : IRequest<SignInCommandResult>
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public class SignInCommandHandler : IRequestHandler<SignInCommand, SignInCommandResult>
        {
            private readonly IIdentityService _identityService;

            public SignInCommandHandler(IIdentityService identityService) => _identityService = identityService;

            public async Task<SignInCommandResult> Handle(SignInCommand request, CancellationToken cancellationToken)
            {
                var user = await _identityService.GetUser(request.Email);
                var isTfaEnabled = _identityService.GetTwoFactorEnabled(user.Id);

                if (isTfaEnabled.Result)
                    return new SignInCommandResult { Id = user.Id, Email = user.Email };

                var result = await _identityService.Authenticate(request.Email, request.Password);
                if (!result.Succeeded) throw new SecurityException();

                var token = await _identityService.GenerateAuthToken(user);
                return new SignInCommandResult { Id = user.Id, Email = user.Email, Token = token };
            }
        }
    }
}
