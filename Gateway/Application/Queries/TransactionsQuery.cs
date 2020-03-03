using AutoMapper;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class TransactionsQuery : IRequest<TransactionsQueryResult>
    {
        public Currency Currency { get; set; }

        public class TransactionsQueryHandler : IRequestHandler<TransactionsQuery, TransactionsQueryResult>
        {
            private readonly IAccountService _accountService;
            private readonly ICurrentUserService _currentUserService;
            private readonly IMapper _mapper;

            public TransactionsQueryHandler(IAccountService accountService, ICurrentUserService currentUserService, IMapper mapper)
                => (_accountService, _currentUserService, _mapper) = (accountService, currentUserService, mapper);

            public async Task<TransactionsQueryResult> Handle(TransactionsQuery request, CancellationToken cancellationToken)
                => new TransactionsQueryResult
                {
                    Transactions = (await _accountService.GetTransactions(_currentUserService.UserId, request.Currency))
                        .Select(x => new TransactionsQueryResult.Transaction { Id = x.Id, DateTime = x.DateTime, Amount = x.Amount, Currency = x.Currency, Balance = x.Balance, Source = x.Source })
                        .ToArray()
                };
        }
    }
}
