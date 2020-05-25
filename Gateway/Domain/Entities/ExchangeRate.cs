using Binebase.Exchange.Common.Domain;
using System;

namespace Binebase.Exchange.Gateway.Domain.Entities
{
    public class ExchangeRate : Common.Domain.AuditableEntity
    {
        public Currency Base { get; set; }
        public Currency Quote { get; set; }
        public decimal Rate { get; set; }
        public DateTime DateTime { get; set; }
    }
}
