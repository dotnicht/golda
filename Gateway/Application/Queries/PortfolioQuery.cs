using Binebase.Exchange.Gateway.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
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
            private readonly ILogger _logger;

            public PortfolioQueryHandler(ICurrentUserService currentUserService, IAccountService accountService, IIdentityService identityService, ILogger<PortfolioQueryHandler> logger)
                => (_currentUserService, _accountService, _identityService, _logger) = (currentUserService, accountService, identityService, logger);

            public async Task<PortfolioQueryResult> Handle(PortfolioQuery request, CancellationToken cancellationToken)
            {
                _logger.LogInformation("Get portfolio for user with Id ='{Id}'",_currentUserService.UserId);
                return new PortfolioQueryResult { Portfolio = await _accountService.GetPorfolio(_currentUserService.UserId), Referrers = await _identityService.GetReferrersCount(_currentUserService.UserId) };
            }
        }
    }
}
