using AutoMapper;
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

namespace Binebase.Exchange.AccountService.Application.Queries
{
    public class TransactionsQuery : IRequest<TransactionsQueryResult>,  IIdContainer
    {
        public Guid Id { get; set; }

        public class TransactionsQueryHandler : IRequestHandler<TransactionsQuery, TransactionsQueryResult>
        {
            private readonly IStoreEvents _storeEvents;
            private readonly IRepository _repository;
            private readonly IMapper _mapper;

            public TransactionsQueryHandler(IStoreEvents storeEvents, IRepository repository, IMapper mapper)
                => (_storeEvents, _repository, _mapper) = (storeEvents, repository, mapper);

            public async Task<TransactionsQueryResult> Handle(TransactionsQuery request, CancellationToken cancellationToken)
            {
                var account = _repository.GetById<Account>(request.Id, int.MaxValue);

                if (!account.Created)
                {
                    throw new NotFoundException(nameof(Account), request.Id);
                }

                using var stream = _storeEvents.OpenStream(request.Id, 0, int.MaxValue);
                var trx = new List<TransactionsQueryResult.Transaction>();
                var balance = Enum.GetNames(typeof(Currency)).Select(x => Enum.Parse<Currency>(x)).ToDictionary(x => x, x => 0M);

                foreach (var commited in stream.CommittedEvents.Where(x => x.Body is AccountDebitedEvent || x.Body is AccountCreditedEvent))
                {
                    var tx = _mapper.Map<TransactionsQueryResult.Transaction>(commited.Body);

                    if (commited.Body is AccountCreditedEvent)
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
}
