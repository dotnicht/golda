
using Binebase.Exchange.Common.Application.Exceptions;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class DisableMultyCommand : IRequest
    {
        public class DisableMultyCommandHandler : IRequestHandler<DisableMultyCommand>
        {
            private readonly IIdentityService _identityService;
            private readonly ICurrentUserService _currentUserService;
            public DisableMultyCommandHandler(IIdentityService identityService, ICurrentUserService currentUserService)
                => (_identityService, _currentUserService) = (identityService, currentUserService);

            public async Task<Unit> Handle(DisableMultyCommand request, CancellationToken cancellationToken)
            {
                var user = await _identityService.GetUser(_currentUserService.UserId);
                if (user == null) throw new NotFoundException(nameof(User), _currentUserService.UserId);

                var status = _identityService.GetTwoFactorEnabled(user.Id);
                if (status.Result) return Unit.Value;//TODO:clarify the behavior with Nicholas

                await _identityService.SetTwoFactorAuthentication(_currentUserService.UserId, false);

                return Unit.Value;
            }
        }
    }
}
