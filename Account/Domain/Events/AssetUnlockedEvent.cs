using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.AccountService.Domain.Events
{
    public class AssetUnlockedEvent
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
    }
}
