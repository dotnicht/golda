using Binebase.Exchange.Common.Application.Exceptions;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class PhoneVerifyStatusQuery : IRequest<PhoneVerifyStatusQueryResult>
    {
        public class PhoneVerifyStatusQueryHandler : IRequestHandler<PhoneVerifyStatusQuery, PhoneVerifyStatusQueryResult>
        {
            private readonly ICurrentUserService _currentUserService;
            private readonly IIdentityService _identityService;
            private readonly IPhoneService _phoneService;

            public PhoneVerifyStatusQueryHandler(IIdentityService identityService, ICurrentUserService currentUserService, IPhoneService phoneService)
                => (_identityService, _currentUserService, _phoneService) = (identityService, currentUserService, phoneService);

            public async Task<PhoneVerifyStatusQueryResult> Handle(PhoneVerifyStatusQuery request, CancellationToken cancellationToken)
            {
                var user = await _identityService.GetUser(_currentUserService.UserId);

                if (user == null)
                {
                    throw new NotFoundException(nameof(User), _currentUserService.UserId);
                }

                if (string.IsNullOrEmpty(user.PhoneNumber))
                {
                    throw new SecurityException(ErrorCode.InvalidPhoneNumber);
                }

                var response = new PhoneVerifyStatusQueryResult { Status = user.PhoneNumberConfirmed };

                return response;
            }
        }
    }
}