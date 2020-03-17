using Binebase.Exchange.Common.Application.Mappings;
using System;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class ExchangeRateQueryResult
    {
        public ExchangeRate[] Rates { get; set; }

        public class ExchangeRate : IMapFrom<Domain.Entities.ExchangeRate>
        {
            public decimal Rate { get; set; }
            public DateTime DateTime { get; set; }
        }
    }
}
