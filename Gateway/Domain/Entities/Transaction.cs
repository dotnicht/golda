using Binebase.Exchange.Common.Domain;
using System;

namespace Binebase.Exchange.Gateway.Domain.Entities
{
    public class Transaction : AuditableEntity
    {
        public Currency Currency { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; } // TODO: remove.
        public DateTime DateTime { get; set; }
        public TransactionType Type { get; set; }
        public string Hash { get; set; }
        public bool Failed { get; set; }
    }
}
