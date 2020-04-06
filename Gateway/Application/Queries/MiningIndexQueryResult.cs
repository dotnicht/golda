using Binebase.Exchange.Common.Application.Mappings;
using Binebase.Exchange.Gateway.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class MiningIndexQueryResult : IMapFrom<MiningRequest>
    {
        public Guid Id { get; set; }
        public int Index { get; set; }
        public decimal Amount { get; set; }
    }
}
