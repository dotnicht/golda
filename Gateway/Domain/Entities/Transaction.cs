using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Domain.Enums;
using System;

namespace Binebase.Exchange.Gateway.Domain.Entities
{
    public class Transaction : AuditableEntity
    {
        public Currency Currency { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public DateTime DateTime { get; set; }
        public TransactionSource Source { get; set; }
        public TransactionType? Type { get; set; }
    }
}
