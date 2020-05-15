using Binebase.Exchange.Gateway.Domain.ValueObjects;
using System;

namespace Binebase.Exchange.Gateway.Domain.Entities
{
    public class ExchangeRate : Common.Domain.AuditableEntity
    {
        public Pair Pair { get; set; }
        public decimal Rate { get; set; }
        public DateTime DateTime { get; set; }
    }
}
