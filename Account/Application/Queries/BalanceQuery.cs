using Binebase.Exchange.AccountService.Domain.Aggregates;
using Binebase.Exchange.AccountService.Domain.Enums;
using Binebase.Exchange.Common.Application.Interfaces;
using MediatR;
using NEventStore.Domain.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.AccountService.Application.Queries
{
    public class BalanceQuery : IRequest<BalanceQueryResult>, IIdContainer
    {
        public Guid Id { get; set; }
        public Currency Currency { get; set; }

        public class BalanceQueryHandler : IRequestHandler<BalanceQuery, BalanceQueryResult>
        {
            private readonly IRepository _repository;

            public BalanceQueryHandler(IRepository repository) => _repository = repository;

            public async Task<BalanceQueryResult> Handle(BalanceQuery request, CancellationToken cancellationToken)
            {
                var account = _repository.GetById<Account>(request.Id, int.MaxValue);
                return await Task.FromResult(new BalanceQueryResult { Amount = account.Balance(request.Currency) });
            }
        }
    }
}
