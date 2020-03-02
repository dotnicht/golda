using Binebase.Exchange.Gateway.Domain.Enums;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class MiningBonusCommandResult
    {
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
    }
}
