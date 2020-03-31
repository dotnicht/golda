using System;

namespace Binebase.Exchange.AccountService.Domain.Events
{
    public class AssetLockedEvent
    {
        public Guid AssetId { get; set; }
        public DateTime DateTime { get; set; }
    }
}
