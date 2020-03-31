using Binebase.Exchange.AccountService.Application.Commands;
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
        IRequestHandler<CreateAccountCommand>,
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

        public async Task<Unit> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var account = Get(request.Id);
            if (account.Exists)
            {
                throw new AccountException(ErrorCode.AccountExists);
            }

            account = new Account(request.Id, _dateTime.UtcNow);
            _repository.Save(account, Guid.NewGuid());
            return await Task.FromResult(Unit.Value);
        }

        public async Task<Unit> Handle(AddAssetCommand request, CancellationToken cancellationToken)
        {
            var account = Get(request.Id);
            account.AddAsset(request.AssetId, request.Currency, _dateTime.UtcNow);
            _repository.Save(account, Guid.NewGuid());
            return await Task.FromResult(Unit.Value);
        }

        public async Task<Unit> Handle(RemoveAssetCommand request, CancellationToken cancellationToken)
        {
            var account = Get(request.Id);
            account.RemoveAsset(request.AssetId, _dateTime.UtcNow);
            _repository.Save(account, Guid.NewGuid());
            return await Task.FromResult(Unit.Value);
        }

        public async Task<Unit> Handle(DebitCommand request, CancellationToken cancellationToken)
        {
            var account = Get(request.Id);
            account.Debit(request.AssetId, request.TransactionId, request.Amount, _dateTime.UtcNow, request.Type);
            _repository.Save(account, Guid.NewGuid());
            return await Task.FromResult(Unit.Value);
        }

        public async Task<Unit> Handle(CreditCommand request, CancellationToken cancellationToken)
        {
            var account = Get(request.Id);
            account.Credit(request.AssetId, request.TransactionId, request.Amount, _dateTime.UtcNow, request.Type);
            _repository.Save(account, Guid.NewGuid());
            return await Task.FromResult(Unit.Value);
        }

        public async Task<Unit> Handle(LockAccountCommand request, CancellationToken cancellationToken)
        {
            var account = Get(request.Id);
            account.Lock(_dateTime.UtcNow);
            _repository.Save(account, Guid.NewGuid());
            return await Task.FromResult(Unit.Value);
        }

        public async Task<Unit> Handle(UnlockAccountCommand request, CancellationToken cancellationToken)
        {
            var account = Get(request.Id);
            account.Unlock(_dateTime.UtcNow);
            _repository.Save(account, Guid.NewGuid());
            return await Task.FromResult(Unit.Value);
        }

        public async Task<Unit> Handle(LockAssetCommand request, CancellationToken cancellationToken)
        {
            var account = Get(request.Id);
            account.Lock(request.AssetId, _dateTime.UtcNow);
            _repository.Save(account, Guid.NewGuid());
            return await Task.FromResult(Unit.Value);
        }

        public async Task<Unit> Handle(UnlockAssetCommand request, CancellationToken cancellationToken)
        {
            var account = Get(request.Id);
            account.Unlock(request.AssetId, _dateTime.UtcNow);
            _repository.Save(account, Guid.NewGuid());
            return await Task.FromResult(Unit.Value);
        }

        private Account Get(Guid id) => _repository.GetById<Account>(id, int.MaxValue);
    }
}
