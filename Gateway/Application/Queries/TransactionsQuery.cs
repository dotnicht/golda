using AutoMapper;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class TransactionsQuery : IRequest<TransactionsQueryResult>
    {
        public Currency? Currency { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; }
        public int ItemsPerPage { get; set; }

        public class TransactionsQueryHandler : IRequestHandler<TransactionsQuery, TransactionsQueryResult>
        {
            private readonly IAccountService _accountService;
            private readonly ICurrentUserService _currentUserService;
            private readonly IMapper _mapper;

            public TransactionsQueryHandler(IAccountService accountService, ICurrentUserService currentUserService, IMapper mapper)
                => (_accountService, _currentUserService, _mapper) = (accountService, currentUserService, mapper);

            public async Task<TransactionsQueryResult> Handle(TransactionsQuery request, CancellationToken cancellationToken)
            {
                var trans = await _accountService.GetTransactions(_currentUserService.UserId);
                var transactions = _mapper.Map<TransactionsQueryResult.Transaction[]>(trans);

                var filteredTransactions = transactions.Where(
                    x => (request.StartDate == null || x.DateTime > request.StartDate) 
                    && (request.EndDate == null || x.DateTime < request.EndDate) 
                    && (request.Currency == null || x.Currency == request.Currency));

                if (request.PageNumber > 0 && request.ItemsPerPage > 0)
                {
                    var startIndex = (request.PageNumber - 1) * request.ItemsPerPage + 1;
                    var endIndex = request.PageNumber * request.ItemsPerPage;
                    var resTrans = filteredTransactions.Skip(startIndex).Take(endIndex - startIndex);

                    return new TransactionsQueryResult { Transactions = resTrans.ToArray() };
                }

                return new TransactionsQueryResult { Transactions = filteredTransactions.ToArray() };
            }
        }
    }
}
