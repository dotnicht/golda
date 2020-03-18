using Binebase.Exchange.Gateway.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.Gateway.Domain.Entities
{
    public class ExchangeOperation : AuditableEntity
    {
        public Pair Pair { get; set; }
        public decimal Amount { get; set; }
    }
}
