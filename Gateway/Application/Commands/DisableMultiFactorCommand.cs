using Binebase.Exchange.Common.Application.Exceptions;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class DisableMultiFactorCommand : IRequest
    {
        public string Password { get; set; }
        public string Code { get; set; }

        public class DisableMultiCommandHandler : IRequestHandler<DisableMultiFactorCommand>
        {
            private readonly IIdentityService _identityService;
            private readonly ICurrentUserService _currentUserService;
            public DisableMultiCommandHandler(IIdentityService identityService, ICurrentUserService currentUserService)
                => (_identityService, _currentUserService) = (identityService, currentUserService);

            public async Task<Unit> Handle(DisableMultiFactorCommand request, CancellationToken cancellationToken)
            {
                var user = await _identityService.GetUser(_currentUserService.UserId);

                if (user == null)
                {
                    throw new NotFoundException(nameof(User), _currentUserService.UserId);
                }

                if (await _identityService.GetTwoFactorEnabled(user.Id))
                {
                    if (!await _identityService.CheckUserPassword(user.Id, request.Password) || !await _identityService.VerifyTwoFactorToken(user.Id, request.Code))
                    {
                        throw new SecurityException(ErrorCode.MultiFactor);
                    }

                    await _identityService.SetTwoFactorAuthentication(_currentUserService.UserId, false);
                }

                return Unit.Value;
            }
        }
    }
}
