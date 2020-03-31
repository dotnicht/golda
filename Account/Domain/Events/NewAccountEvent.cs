using System;

namespace Binebase.Exchange.AccountService.Domain.Events
{
    public class NewAccountEvent
    {  
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
    }
}
