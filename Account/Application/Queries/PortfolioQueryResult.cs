using Binebase.Exchange.AccountService.Domain.Enums;
using System.Collections.Generic;

namespace Binebase.Exchange.AccountService.Application.Queries
{
    public class PortfolioQueryResult
    {
        public Dictionary<Currency, decimal> Portfolio { get; set; }
    }
}