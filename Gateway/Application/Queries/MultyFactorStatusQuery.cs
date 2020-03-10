using Binebase.Exchange.Common.Application.Exceptions;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class MultyFactorStatusQuery : IRequest<MultyFactorStatusQueryResult>
    {
        public class MultyFactorStatusQueryHandler : IRequestHandler<MultyFactorStatusQuery, MultyFactorStatusQueryResult>
        {
            private readonly ICurrentUserService _currentUserService;
            private readonly IIdentityService _identityService;

            public MultyFactorStatusQueryHandler(IIdentityService identityService, ICurrentUserService currentUserService)
                => (_identityService, _currentUserService) = (identityService, currentUserService);

            public async Task<MultyFactorStatusQueryResult> Handle(MultyFactorStatusQuery request, CancellationToken cancellationToken)
            {
                var user = await _identityService.GetUser(_currentUserService.UserId);

                if (user == null)
                {
                    throw new NotFoundException(nameof(User), _currentUserService.UserId);
                }

                var status = await _identityService.GetTwoFactorEnabled(user.Id);
                var key = null as string;
                var uri = null as string;

                if (!status)
                {
                    key = await _identityService.GetAuthenticatorKey(_currentUserService.UserId);

                    if (string.IsNullOrEmpty(key))
                    {
                        await _identityService.ResetAuthenticatorKey(_currentUserService.UserId);
                        key = await _identityService.GetAuthenticatorKey(_currentUserService.UserId);
                    }

                    uri = await _identityService.GenerateAuthenticatorUrl(user, key);
                }

                return await Task.FromResult(new MultyFactorStatusQueryResult { Status = status, Code = key, Url = uri });
            }
        }
    }
}
