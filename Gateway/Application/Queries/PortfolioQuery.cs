using Binebase.Exchange.Gateway.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class PortfolioQuery : IRequest<PortfolioQueryResult>
    {
        public class PortfolioQueryHandler : IRequestHandler<PortfolioQuery, PortfolioQueryResult>
        {
            private readonly ICurrentUserService _currentUserService;
            private readonly IAccountService _accountService;
            private readonly IIdentityService _identityService;

            public PortfolioQueryHandler(ICurrentUserService currentUserService, IAccountService accountService, IIdentityService identityService)
                => (_currentUserService, _accountService, _identityService) = (currentUserService, accountService, identityService);

            public async Task<PortfolioQueryResult> Handle(PortfolioQuery request, CancellationToken cancellationToken)
                => new PortfolioQueryResult { Portfolio = await _accountService.GetPorfolio(_currentUserService.UserId), Referrers = await _identityService.GetReferrersCount(_currentUserService.UserId) };
        }
    }
}
