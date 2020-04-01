using Binebase.Exchange.AccountService.Contracts.Commands;
using Binebase.Exchange.AccountService.Domain;
using Binebase.Exchange.AccountService.Domain.Aggregates;
using Binebase.Exchange.AccountService.Domain.Exceptions;
using Binebase.Exchange.Common.Application.Interfaces;
using MediatR;
using NEventStore.Domain.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.AccountService.Application
{
    public class CommandHandler :
        IRequestHandler<NewAccountCommand>,
        IRequestHandler<AddAssetCommand>,
        IRequestHandler<RemoveAssetCommand>,
        IRequestHandler<DebitCommand>,
        IRequestHandler<CreditCommand>,
        IRequestHandler<LockAccountCommand>,
        IRequestHandler<UnlockAccountCommand>,
        IRequestHandler<LockAssetCommand>,
        IRequestHandler<UnlockAssetCommand>
    {
        private readonly IRepository _repository;
        private readonly IDateTime _dateTime;

        public CommandHandler(IRepository repository, IDateTime dateTime) => (_repository, _dateTime) = (repository, dateTime);

        public async Task<Unit> Handle(NewAccountCommand request, CancellationToken cancellationToken)
        {
            var account = _repository.GetById<Account>(request.Id, int.MaxValue);
            if (account.Exists)
            {
                throw new AccountException(ErrorCode.AccountExists);
            }

            account = new Account(request.Id, _dateTime.UtcNow);
            _repository.Save(account, Guid.NewGuid());
            return await Task.FromResult(Unit.Value);
        }

        public async Task<Unit> Handle(AddAssetCommand request, CancellationToken cancellationToken)
            => await Action(request.Id, x => x.AddAsset(request.AssetId, request.Currency, _dateTime.UtcNow));

        public async Task<Unit> Handle(RemoveAssetCommand request, CancellationToken cancellationToken)
            => await Action(request.Id, x => x.RemoveAsset(request.AssetId, _dateTime.UtcNow));

        public async Task<Unit> Handle(DebitCommand request, CancellationToken cancellationToken)
            => await Action(request.Id, x => x.Debit(request.AssetId, request.TransactionId, request.Amount, _dateTime.UtcNow, request.Type));

        public async Task<Unit> Handle(CreditCommand request, CancellationToken cancellationToken)
            => await Action(request.Id, x => x.Credit(request.AssetId, request.TransactionId, request.Amount, _dateTime.UtcNow, request.Type));

        public async Task<Unit> Handle(LockAccountCommand request, CancellationToken cancellationToken)
           => await Action(request.Id, x => x.Lock(_dateTime.UtcNow));

        public async Task<Unit> Handle(UnlockAccountCommand request, CancellationToken cancellationToken)
            => await Action(request.Id, x => x.Unlock(_dateTime.UtcNow));

        public async Task<Unit> Handle(LockAssetCommand request, CancellationToken cancellationToken)
            => await Action(request.Id, x => x.Lock(request.AssetId, _dateTime.UtcNow));

        public async Task<Unit> Handle(UnlockAssetCommand request, CancellationToken cancellationToken)
            => await Action(request.Id, x => x.Unlock(request.AssetId, _dateTime.UtcNow));

        private async Task<Unit> Action(Guid id, Action<Account> action)
        {
            var account = _repository.GetById<Account>(id, int.MaxValue);
            action(account);
            _repository.Save(account, Guid.NewGuid());
            return await Task.FromResult(Unit.Value);
        }
    }
}
