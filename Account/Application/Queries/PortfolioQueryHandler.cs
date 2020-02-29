using Binebase.Exchange.AccountService.Application.Common.Exceptions;
using Binebase.Exchange.AccountService.Domain.Aggregates;
using MediatR;
using NEventStore.Domain.Persistence;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.AccountService.Application.Queries
{
    public class PortfolioQueryHandler : IRequestHandler<PortfolioQuery, PortfolioQueryResult>
    {
        private readonly IRepository _repository;

        public PortfolioQueryHandler(IRepository repository) => _repository = repository;

        public async Task<PortfolioQueryResult> Handle(PortfolioQuery request, CancellationToken cancellationToken)
        {
            var account = _repository.GetById<Account>(request.Id) ?? throw new NotFoundException(nameof(Account), request.Id);
            var result = new PortfolioQueryResult { Portfolio = account.Portfolio.ToDictionary(x => x.Currency, x => x.Amount) };
            return await Task.FromResult(result);
        }
    }
}
