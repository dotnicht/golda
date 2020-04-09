using AutoMapper;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Application.Queries
{
    public class TransactionsQuery : IRequest<TransactionsQueryResult>
    {
        public Guid Id { get; set; }

        public class TransactionsQueryHandler : IRequestHandler<TransactionsQuery, TransactionsQueryResult>
        {
            private readonly IApplicationDbContext _context;
            private readonly IMapper _mapper;

            public TransactionsQueryHandler(IApplicationDbContext context, IMapper mapper) => (_context, _mapper) = (context, mapper);

            public async Task<TransactionsQueryResult> Handle(TransactionsQuery request, CancellationToken cancellationToken)
            {
                var tx = _context.Transactions.Include(x => x.Address).Where(x => x.Address.AccountId == request.Id).OrderByDescending(x => x.Created);
                return await Task.FromResult(new TransactionsQueryResult { Transactions = _mapper.Map<TransactionsQueryResult.Transaction[]>(tx) });
            }
        }
    }
}
