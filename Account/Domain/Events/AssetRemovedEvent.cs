using System;

namespace Binebase.Exchange.AccountService.Domain.Events
{
    public class AssetRemovedEvent
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
    }
}
