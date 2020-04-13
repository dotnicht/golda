using Binebase.Exchange.Common.Domain;
using System;

namespace Binebase.Exchange.Gateway.Domain.Entities
{
    public class Transaction : AuditableEntity
    {
        public Currency Currency { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public DateTime DateTime { get; set; }
        public TransactionType Type { get; set; }
        public string Hash { get; set; }
    }
}
