using Binebase.Exchange.AccountService.Application.Common.Exceptions;
using Binebase.Exchange.AccountService.Domain.Aggregates;
using Binebase.Exchange.AccountService.Domain.Common;
using MediatR;
using NEventStore.Domain.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.AccountService.Application.Common
{
    public abstract class AccountHandlerBase<TRequest> : AccountHandlerBase<TRequest, Unit> where TRequest : IRequest<Unit>, IIdContainer
    {
        protected AccountHandlerBase(IRepository repository) : base(repository) { }
    }

    public abstract class AccountHandlerBase<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>, IIdContainer
    {
        protected IRepository Repository { get; private set; }

        protected AccountHandlerBase(IRepository repository) => Repository = repository;

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            var account = Repository.GetById<Account>(request.Id);
            if (!account.Created)
            {
                throw new NotFoundException(nameof(Account), request.Id);
            }

            return await Handle(account, request, cancellationToken);
        }

        public abstract Task<TResponse> Handle(Account account, TRequest request, CancellationToken cancellationToken);
    }
}
