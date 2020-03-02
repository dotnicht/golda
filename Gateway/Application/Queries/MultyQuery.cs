using Binebase.Exchange.Common.Application.Exceptions;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using MediatR;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class MultyQuery : IRequest<MultyQueryResult>
    {
        public class MultyQueryHandler : IRequestHandler<MultyQuery, MultyQueryResult>
        {
            private readonly ICurrentUserService _currentUserService;
            private readonly IIdentityService _identityService;

            public MultyQueryHandler(IIdentityService identityService, ICurrentUserService currentUserService)
                => (_identityService, _currentUserService) = (identityService, currentUserService);

            public async Task<MultyQueryResult> Handle(MultyQuery request, CancellationToken cancellationToken)
            {
                var user = await _identityService.GetUser(_currentUserService.UserId);
                if (user == null) throw new NotFoundException(nameof(User), _currentUserService.UserId);

                var status = await _identityService.GetTwoFactorEnabled(user.Id);
                if (status) return await Task.FromResult(new MultyQueryResult { Status = status });

                var key = await _identityService.GetAuthenticatorKey(_currentUserService.UserId);
                if (string.IsNullOrEmpty(key))
                {
                    await _identityService.ResetAuthenticatorKey(_currentUserService.UserId);
                    key = await _identityService.GetAuthenticatorKey(_currentUserService.UserId);
                }

                var authenticatorUri = await _identityService.GenerateAuthenticatorUrl(user, key);

                return await Task.FromResult(new MultyQueryResult { Status = false, Code = key, Url = authenticatorUri });
            }
        }
    }
}
