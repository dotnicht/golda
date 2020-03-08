using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Domain.Enums;

namespace Binebase.Exchange.Gateway.Domain.Entities
{
    public class MiningRequest : AuditableEntity
    {
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
    }
}
