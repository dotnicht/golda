using AutoMapper;
using Binebase.Exchange.AccountService.Application.Queries;
using Binebase.Exchange.AccountService.Domain.Aggregates;
using Binebase.Exchange.AccountService.Domain.Events;
using Binebase.Exchange.Common.Application.Exceptions;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using MediatR;
using NEventStore;
using NEventStore.Domain.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.AccountService.Application
{
    public class QueryHandler :
        IRequestHandler<BalanceQuery, BalanceQueryResult>,
        IRequestHandler<PortfolioQuery, PortfolioQueryResult>,
        IRequestHandler<TransactionsQuery, TransactionsQueryResult>
    {
        private readonly IRepository _repository;
        private IStoreEvents _storeEvents;
        private readonly IDateTime _dateTime;
        private IMapper _mapper;

        public QueryHandler(IRepository repository, IStoreEvents storeEvents, IDateTime dateTime, IMapper mapper) 
            => (_repository, _storeEvents, _dateTime, _mapper) = (repository, storeEvents, dateTime, mapper);

        public async Task<BalanceQueryResult> Handle(BalanceQuery request, CancellationToken cancellationToken)
        {
            var account = _repository.GetById<Account>(request.Id, int.MaxValue);
            return await Task.FromResult(new BalanceQueryResult { Amount = account.Balance(request.AssetId) });
        }

        public async Task<PortfolioQueryResult> Handle(PortfolioQuery request, CancellationToken cancellationToken)
        {
            var account = _repository.GetById<Account>(request.Id, int.MaxValue);
            var result = new PortfolioQueryResult { Portfolio = account.Portfolio.ToArray() };
            return await Task.FromResult(result);
        }

        public async Task<TransactionsQueryResult> Handle(TransactionsQuery request, CancellationToken cancellationToken)
        {
            var account = _repository.GetById<Account>(request.Id, int.MaxValue);

            if (!account.Exists)
            {
                throw new NotFoundException(nameof(Account), request.Id);
            }

            using var stream = _storeEvents.OpenStream(request.Id, 0, int.MaxValue);
            var trx = new List<TransactionsQueryResult.Transaction>();
            foreach (var commited in stream.CommittedEvents.Where(x => x.Body is DebitedEvent || x.Body is CreditedEvent))
            {
                var tx = _mapper.Map<TransactionsQueryResult.Transaction>(commited.Body);

                if (commited.Body is CreditedEvent)
                {
                    tx.Amount = -tx.Amount;
                }

                trx.Add(tx);
            }

            return await Task.FromResult(new TransactionsQueryResult { Transactions = trx.ToArray() });
        }
    }
}
