using Binebase.Exchange.AccountService.Domain.Aggregates;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using MediatR;
using NEventStore.Domain.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.AccountService.Application.Commands
{
    public class DebitAccountCommand : IRequest<DebitAccountCommandResult>, IIdContainer
    {
        public Guid Id { get; set; } // TODO: add transaction id.
        public Currency Currency { get; set; }
        public decimal Amount { get; set; }
        public string Payload { get; set; }

        public class DebitAccountCommandHandler : IRequestHandler<DebitAccountCommand, DebitAccountCommandResult>
        {
            private readonly IRepository _repository;

            public DebitAccountCommandHandler(IRepository repository)
                => _repository = repository;

            public async Task<DebitAccountCommandResult> Handle(DebitAccountCommand request, CancellationToken cancellationToken)
            {
                var account = _repository.GetById<Account>(request.Id, int.MaxValue);
                var id = account.Debit(request.Currency, request.Amount, request.Payload);
                _repository.Save(account, Guid.NewGuid());
                return await Task.FromResult(new DebitAccountCommandResult { Id = id });
            }
        }
    }
}
