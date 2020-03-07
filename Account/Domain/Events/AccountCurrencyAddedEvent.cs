using Binebase.Exchange.AccountService.Domain.Enums;
using System;

namespace Binebase.Exchange.AccountService.Domain.Events
{
    public class AccountCurrencyAddedEvent
    {
        public DateTime DateTime { get; set; }
        public Currency Currency { get; set; }
    }
}
