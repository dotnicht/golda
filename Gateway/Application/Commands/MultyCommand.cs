using Binebase.Exchange.Common.Application.Exceptions;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class MultyCommand : IRequest
    {
        public string Code { get; set; }
        public class MultyCommandHandler : IRequestHandler<MultyCommand>
        {
            private readonly IIdentityService _identityService;
            private readonly ICurrentUserService _currentUserService;

            public MultyCommandHandler(IIdentityService identityService, ICurrentUserService currentUserService)
                => (_identityService, _currentUserService) = (identityService, currentUserService);

            public async Task<Unit> Handle(MultyCommand request, CancellationToken cancellationToken)
            {
                var user = await _identityService.GetUser(_currentUserService.UserId);
                if (user == null) throw new NotFoundException(nameof(User), _currentUserService.UserId);

                var status = _identityService.GetTwoFactorEnabled(user.Id);
                if (status.Result) return Unit.Value;//TODO:clarify the behavior with Nicholas

                var code = request.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
                var valid = await _identityService.VerifyTwoFactorToken(_currentUserService.UserId, code);
                if (!valid) throw new NotSupportedException();

                await _identityService.EnableTwoFactorAuthentication(_currentUserService.UserId, true);

                return Unit.Value;
            }
        }
    }
}
