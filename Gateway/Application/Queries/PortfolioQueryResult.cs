using Binebase.Exchange.Common.Domain;
using System.Collections.Generic;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class PortfolioQueryResult
    {
        public Dictionary<Currency, decimal> Portfolio { get; set; }
        public int Referrers { get; set; } 
    }
}