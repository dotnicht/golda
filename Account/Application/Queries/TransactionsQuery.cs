using Binebase.Exchange.AccountService.Domain.Aggregates;
using Binebase.Exchange.AccountService.Domain.Common;
using Binebase.Exchange.AccountService.Domain.Enums;
using Binebase.Exchange.AccountService.Domain.Events;
using Binebase.Exchange.Common.Application.Exceptions;
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
        public Currency Currency { get; set; }

        public class TransactionsQueryHandler : IRequestHandler<TransactionsQuery, TransactionsQueryResult>
        {
            private readonly IStoreEvents _storeEvents;
            private readonly IRepository _repository;

            public TransactionsQueryHandler(IStoreEvents storeEvents, IRepository repository)
            {
                _storeEvents = storeEvents;
                _repository = repository;
            }

            public async Task<TransactionsQueryResult> Handle(TransactionsQuery request, CancellationToken cancellationToken)
            {
                var account = _repository.GetById<Account>(request.Id, int.MaxValue);
                if (!account.Created) throw new NotFoundException(nameof(Account), request.Id);

                using var stream = _storeEvents.OpenStream(request.Id, 0, int.MaxValue);
                var trx = new List<TransactionsQueryResult.Transaction>();
                var balance = 0M;

                foreach (var commited in stream.CommittedEvents.Where(x => x.Body is ITransaction t && t.Currency == request.Currency))
                {
                    var tx = new TransactionsQueryResult.Transaction
                    {
                        Id = ((IIdContainer)commited.Body).Id,
                        DateTime = ((IDateTimeContainer)commited.Body).DateTime
                    };

                    switch (commited.Body)
                    {
                        case AccountDebitedEvent debit:
                            tx.Amount = debit.Amount;
                            balance += debit.Amount;
                            tx.Balance = balance;
                            tx.Payload = debit.Payload;
                            break;
                        case AccountCreditedEvent credit:
                            tx.Amount = -credit.Amount;
                            balance -= credit.Amount;
                            tx.Balance = balance;
                            tx.Payload = credit.Payload;
                            break;
                        default: throw new NotSupportedException();
                    }

                    trx.Add(tx);
                }

                return await Task.FromResult(new TransactionsQueryResult { Transactions = trx.ToArray() });
            }
        }
    }
}
