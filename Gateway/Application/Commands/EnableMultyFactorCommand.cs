using Binebase.Exchange.Common.Application.Exceptions;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class EnableMultyFactorCommand : IRequest
    {
        public string Code { get; set; }

        public class MultyCommandHandler : IRequestHandler<EnableMultyFactorCommand>
        {
            private readonly IIdentityService _identityService;
            private readonly ICurrentUserService _currentUserService;

            public MultyCommandHandler(IIdentityService identityService, ICurrentUserService currentUserService)
                => (_identityService, _currentUserService) = (identityService, currentUserService);

            public async Task<Unit> Handle(EnableMultyFactorCommand request, CancellationToken cancellationToken)
            {
                var user = await _identityService.GetUser(_currentUserService.UserId);

                if (user == null)
                {
                    throw new NotFoundException(nameof(User), _currentUserService.UserId);
                }

                if (!await _identityService.GetTwoFactorEnabled(user.Id))
                {
                    var code = request.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
                    if (!await _identityService.VerifyTwoFactorToken(_currentUserService.UserId, code))
                    {
                        throw new SecurityException();
                    }

                    await _identityService.SetTwoFactorAuthentication(_currentUserService.UserId, true);
                }

                return Unit.Value;
            }
        }
    }
}
