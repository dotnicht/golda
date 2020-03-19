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

                var response = new MultyFactorStatusQueryResult { Status = await _identityService.GetTwoFactorEnabled(user.Id) };

                if (!response.Status)
                {
                    (response.Code, response.Url) = await _identityService.GetAuthenticatorData(user);
                }

                return response;
            }
        }
    }
}
