using Binebase.Exchange.AccountService.Domain.Aggregates;
using Binebase.Exchange.AccountService.Domain.Common;
using Binebase.Exchange.AccountService.Domain.Exceptions;
using MediatR;
using NEventStore.Domain.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.AccountService.Application.Commands
{
    public class CreateAccountCommand : IRequest, IIdContainer
    {
        public Guid Id { get; set; }

        public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand>
        {
            private readonly IRepository _repository;

            public CreateAccountCommandHandler(IRepository repository) => _repository = repository;

            public async Task<Unit> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
            {
                var account = _repository.GetById<Account>(request.Id, int.MaxValue);
                if (account.Created)
                {
                    throw new AccountException($"Account already exists {request.Id}.");
                }

                account = new Account(request.Id);
                _repository.Save(account, Guid.NewGuid());
                return await Task.FromResult(Unit.Value);
            }
        }
    }
}
