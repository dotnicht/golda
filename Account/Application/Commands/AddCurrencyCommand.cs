using Binebase.Exchange.AccountService.Domain.Aggregates;
using Binebase.Exchange.AccountService.Domain.Enums;
using Binebase.Exchange.Common.Application.Interfaces;
using MediatR;
using NEventStore.Domain.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.AccountService.Application.Commands
{
    public class AddCurrencyCommand : IRequest, IIdContainer
    {
        public Guid Id { get; set; }
        public Currency Currency { get; set; }

        public class AddCurrencyCommandHandler : IRequestHandler<AddCurrencyCommand>
        {
            private readonly IRepository _repository;

            public AddCurrencyCommandHandler(IRepository repository)
                => _repository = repository;

            public async Task<Unit> Handle(AddCurrencyCommand request, CancellationToken cancellationToken)
            {
                var account = _repository.GetById<Account>(request.Id, int.MaxValue);
                account.AddCurrency(request.Currency);
                _repository.Save(account, Guid.NewGuid());
                return await Task.FromResult(Unit.Value);
            }
        }
    }
}