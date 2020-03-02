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

                var status = _identityService.GetTwoFactorEnabled(user.Id);
                if (status.Result) return await Task.FromResult(new MultyQueryResult { Status = status.Result });

                var key = await _identityService.GetAuthenticatorKey(_currentUserService.UserId);
                if (string.IsNullOrEmpty(key))
                {
                    await _identityService.ResetAuthenticatorKey(_currentUserService.UserId);
                    key = await _identityService.GetAuthenticatorKey(_currentUserService.UserId);
                }

                var result = new StringBuilder();
                var index = 0;
                while (index + 4 < key.Length)
                {
                    result.Append(key.Substring(index, 4)).Append(" ");
                    index += 4;
                }

                if (index < key.Length)
                    result.Append(key.Substring(index));

                var SharedKey = result.ToString().ToLowerInvariant();
                var AuthenticatorUriResul = _identityService.GenerateAuthenticatorUrl(user, key);

                return await Task.FromResult(new MultyQueryResult { Status = false, Code = SharedKey, Url = AuthenticatorUriResul.Result });
            }
        }
    }
}
