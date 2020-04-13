using Binebase.Exchange.Gateway.Domain.Enums;

namespace Binebase.Exchange.Gateway.Domain.Entities
{
    public class MiningRequest : AuditableEntity
    {
        public decimal Amount { get; set; }
        public MiningType Type { get; set; }
        public bool IsAnonymous { get; set; }
        public int Index { get; set; }
    }
}
