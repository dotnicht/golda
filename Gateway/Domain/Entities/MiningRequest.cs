using Binebase.Exchange.Gateway.Common.Domain;

namespace Binebase.Exchange.Gateway.Domain.Entities
{
    public class MiningRequest : AuditableEntity
    {
        public decimal Amount { get; set; }
    }
}
