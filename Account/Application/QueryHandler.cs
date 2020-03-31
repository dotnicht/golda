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
using System.Text;
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
        private readonly IDateTime _dateTime;
        private IMapper _mapper;

        public QueryHandler(IRepository repository, IDateTime dateTime, IMapper mapper) => (_repository, _dateTime, _mapper) = (repository, dateTime, mapper);

        public async Task<BalanceQueryResult> Handle(BalanceQuery request, CancellationToken cancellationToken)
        {
            var account = _repository.GetById<Account>(request.Id, int.MaxValue);
            return await Task.FromResult(new BalanceQueryResult { Amount = account.Balance(request.Currency) });
        }

        public async Task<PortfolioQueryResult> Handle(PortfolioQuery request, CancellationToken cancellationToken)
        {
            var account = _repository.GetById<Account>(request.Id, int.MaxValue);
            var result = new PortfolioQueryResult { Portfolio = account.Portfolio.ToDictionary(x => x.Currency, x => x.Amount) };
            return await Task.FromResult(result);
        }

        public async Task<TransactionsQueryResult> Handle(TransactionsQuery request, CancellationToken cancellationToken)
        {
            var account = _repository.GetById<Account>(request.Id, int.MaxValue);

            if (!account.Exists)
            {
                throw new NotFoundException(nameof(Account), request.Id);
            }

            using var stream = IStoreEvents.OpenStream(request.Id, 0, int.MaxValue);
            var trx = new List<TransactionsQueryResult.Transaction>();
            var balance = Enum.GetNames(typeof(Currency)).Select(x => Enum.Parse<Currency>(x)).ToDictionary(x => x, x => 0M);

            foreach (var commited in stream.CommittedEvents.Where(x => x.Body is DebitedEvent || x.Body is CreditedEvent))
            {
                var tx = _mapper.Map<TransactionsQueryResult.Transaction>(commited.Body);

                if (commited.Body is CreditedEvent)
                {
                    tx.Amount = -tx.Amount;
                }

                balance[tx.Currency] += tx.Amount;
                tx.Balance = balance[tx.Currency];

                trx.Add(tx);
            }

            return await Task.FromResult(new TransactionsQueryResult { Transactions = trx.ToArray() });
        }
    }
}
