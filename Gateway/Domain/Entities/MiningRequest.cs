using Binebase.Exchange.Gateway.Domain.Enums;

namespace Binebase.Exchange.Gateway.Domain.Entities
{
    public class MiningRequest : AuditableEntity
    {
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public TransactionType Type { get; set; }
        public bool IsAnonymous { get; set; }
    }
}
