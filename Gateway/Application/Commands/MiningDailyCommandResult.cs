using Binebase.Exchange.Gateway.Domain.Enums;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class MiningDailyCommandResult
    {
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
    }
}
