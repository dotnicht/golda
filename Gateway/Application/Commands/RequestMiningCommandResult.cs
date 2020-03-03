using Binebase.Exchange.Common.Application.Mappings;
using Binebase.Exchange.Gateway.Domain.Entities;
using System;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class RequestMiningCommandResult : IMapFrom<MiningRequest>
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
    }
}
