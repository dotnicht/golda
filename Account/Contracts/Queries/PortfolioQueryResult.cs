using Binebase.Exchange.AccountService.Domain.Entities;

namespace Binebase.Exchange.AccountService.Application.Queries
{
    public class PortfolioQueryResult
    {
        public Asset[] Portfolio { get; set; }
    }
}