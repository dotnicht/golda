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
    public class CreditAccountCommand : IRequest<CreditAccountCommandResult>, IIdContainer
    {
        public Guid Id { get; set; }
        public Currency Currency { get; set; }
        public decimal Amount { get; set; }
        public string Payload { get; set; }

        public class CreditAccountCommandHandler : IRequestHandler<CreditAccountCommand, CreditAccountCommandResult>
        {
            private readonly IRepository _repository;

            public CreditAccountCommandHandler(IRepository repository)
                => _repository = repository;

            public async Task<CreditAccountCommandResult> Handle(CreditAccountCommand request, CancellationToken cancellationToken)
            {
                var account = _repository.GetById<Account>(request.Id, int.MaxValue);
                var id = account.Credit(request.Currency, request.Amount, request.Payload);
                _repository.Save(account, Guid.NewGuid());
                return await Task.FromResult(new CreditAccountCommandResult { Id = id });
            }
        }
    }
}
