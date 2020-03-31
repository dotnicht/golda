using Binebase.Exchange.Common.Application.Mappings;
using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.Enums;
using System;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class MiningsQueryResult
    {
        public Mining[] Minings { get; set; }

        public class Mining : IMapFrom<MiningRequest>
        {
            public Guid Id { get; set; }
            public DateTime Created { get; set; }
            public decimal Amount { get; set; }
            public decimal Balance { get; set; }
            public MiningType Type { get; set; }
        }
    }
}
