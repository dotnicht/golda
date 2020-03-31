using Binebase.Exchange.Common.Domain;
using System;

namespace Binebase.Exchange.AccountService.Domain.Events
{
    public class CreditedEvent
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
    }
}
