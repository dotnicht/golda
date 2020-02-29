using Binebase.Exchange.AccountService.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.AccountService.Domain.Events
{
    public class AccountCreatedEvent : IIdContainer, IDateTimeContainer
    {  
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
    }
}
