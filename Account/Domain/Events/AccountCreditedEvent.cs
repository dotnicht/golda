using Binebase.Exchange.AccountService.Domain.Enums;
using System;

namespace Binebase.Exchange.AccountService.Domain.Events
{
    public class AccountCreditedEvent
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public Currency Currency { get; set; }
        public decimal Amount { get; set; }
        public string Payload { get; set; }
    }
}
