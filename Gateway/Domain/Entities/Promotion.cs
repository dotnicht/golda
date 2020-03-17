using Binebase.Exchange.Common.Domain;
using System;

namespace Binebase.Exchange.Gateway.Domain.Entities
{
    public class Promotion : AuditableEntity
    {
        public Currency Currency { get; set; }
        public decimal TokenAmount { get; set; }
        public decimal CurrencyAmount { get; set; }
        public bool IsExchanged { get; set; }
        public Guid MiningRequestId { get; set; }
        public MiningRequest MiningRequest { get; set; }
    }
}
