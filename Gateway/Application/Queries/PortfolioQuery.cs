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

            public PortfolioQueryHandler(ICurrentUserService currentUserService, IAccountService accountService)
                => (_currentUserService, _accountService) = (currentUserService, accountService);

            public async Task<PortfolioQueryResult> Handle(PortfolioQuery request, CancellationToken cancellationToken)
                => new PortfolioQueryResult { Portfolio = await _accountService.GetPorfolio(_currentUserService.UserId) };
        }
    }
}
