using Binebase.Exchange.Common.Application.Exceptions;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using MediatR;
using System;
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

                var status = await _identityService.GetTwoFactorEnabled(user.Id);

                if (status)
                {
                    throw new NotSupportedException();
                }

                var code = request.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
                var valid = await _identityService.VerifyTwoFactorToken(_currentUserService.UserId, code);

                if (!valid)
                {
                    throw new NotSupportedException();
                }

                await _identityService.SetTwoFactorAuthentication(_currentUserService.UserId, true);

                return Unit.Value;
            }
        }
    }
}
