using Binebase.Exchange.Gateway.Domain.ValueObjects;

namespace Binebase.Exchange.Gateway.Domain.Entities
{
    public class ExchangeOperation : AuditableEntity
    {
        public Pair Pair { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal QuoteAmount { get; set; }
    }
}
