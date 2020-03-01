using Binebase.Exchange.AccountService.Domain.Aggregates;
using Binebase.Exchange.AccountService.Domain.Common;
using Binebase.Exchange.AccountService.Domain.Enums;
using MediatR;
using NEventStore.Domain.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.AccountService.Application.Commands
{
    public class RemoveCurrencyCommand : IRequest, IIdContainer
    {
        public Guid Id { get; set; }
        public Currency Currency { get; set; }

        public class RemoveCurrencyCommandHandler : IRequestHandler<RemoveCurrencyCommand>
        {
            private readonly IRepository _repository;

            public RemoveCurrencyCommandHandler(IRepository repository)
                => _repository = repository;

            public async Task<Unit> Handle(RemoveCurrencyCommand request, CancellationToken cancellationToken)
            {
                var account = _repository.GetById<Account>(request.Id, int.MaxValue);
                account.RemoveCurrency(request.Currency);
                _repository.Save(account, Guid.NewGuid());
                return await Task.FromResult(Unit.Value);
            }
        }
    }
}
