using AutoMapper;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Common.Domain;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

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
                    Transactions = _mapper.Map<TransactionsQueryResult.Transaction[]>(await _accountService.GetTransactions(_currentUserService.UserId, request.Currency))
                };
        }
    }
}
