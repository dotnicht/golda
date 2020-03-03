using Binebase.Exchange.Common.Application.Exceptions;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class DisableMultyFactorCommand : IRequest
    {
        public string Password { get; set; }
        public string Code { get; set; }

        public class DisableMultyCommandHandler : IRequestHandler<DisableMultyFactorCommand>
        {
            private readonly IIdentityService _identityService;
            private readonly ICurrentUserService _currentUserService;
            public DisableMultyCommandHandler(IIdentityService identityService, ICurrentUserService currentUserService)
                => (_identityService, _currentUserService) = (identityService, currentUserService);

            public async Task<Unit> Handle(DisableMultyFactorCommand request, CancellationToken cancellationToken)
            {
                var user = await _identityService.GetUser(_currentUserService.UserId);
                if (user == null) throw new NotFoundException(nameof(User), _currentUserService.UserId);

                var status = await _identityService.GetTwoFactorEnabled(user.Id);
                if (!status) throw new NotSupportedException();

                var passStatus = await _identityService.CheckUserPassword(user.Id, request.Password);
                if (!passStatus) throw new NotSupportedException();

                var isTfaValid = await _identityService.VerifyTwoFactorToken(user.Id, request.Code);
                if (!isTfaValid)
                    throw new SecurityException();

                await _identityService.SetTwoFactorAuthentication(_currentUserService.UserId, false);

                return Unit.Value;
            }
        }
    }
}
