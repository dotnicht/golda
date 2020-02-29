using Binebase.Exchange.Common.Domain;

namespace Binebase.Exchange.Gateway.Domain.Entities
{
    public class MiningRequest : AuditableEntity
    {
        public decimal Amount { get; set; }
    }
}
