using Binebase.Exchange.Common.Application.Mappings;
using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.Enums;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class MiningBonusCommandResult : IMapFrom<MiningRequest>
    {
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
    }
}
