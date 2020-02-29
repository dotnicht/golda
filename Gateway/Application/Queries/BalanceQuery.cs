using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Common.Domain;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class BalanceQuery : IRequest<BalanceQueryResult>
    {
        public Currency Currency { get; set; }

        public class BalanceQueryHandler : IRequestHandler<BalanceQuery, BalanceQueryResult>
        {
            private readonly ICurrentUserService _currentUserService;
            private readonly IAccountService _accountService;

            public BalanceQueryHandler(ICurrentUserService currentUserService, IAccountService accountService)
                => (_currentUserService, _accountService) = (currentUserService, accountService);

            public async Task<BalanceQueryResult> Handle(BalanceQuery request, CancellationToken cancellationToken)
                => await Task.FromResult(new BalanceQueryResult { Balance = await _accountService.GetBalance(_currentUserService.UserId, request.Currency) });
        }
    }
}
