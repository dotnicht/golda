using Binebase.Exchange.Common.Application.Exceptions;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using MediatR;
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
                if (user == null)
                {
                    throw new NotFoundException(nameof(User), request.Email);
                }

                var result = new SignInCommandResult { Id = user.Id, Email = user.Email };
                if (!await _identityService.GetTwoFactorEnabled(user.Id))
                {
                    var auth = await _identityService.Authenticate(request.Email, request.Password);
                    if (!auth.Succeeded)
                    {
                        throw auth.ToSecurityException();
                    }

                    result.Token = await _identityService.GenerateAuthToken(user);
                }

                return result;
            }
        }
    }
}
