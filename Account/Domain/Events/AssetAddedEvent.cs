using Binebase.Exchange.Common.Domain;
using System;

namespace Binebase.Exchange.AccountService.Domain.Events
{
    public class AssetAddedEvent
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public Currency Currency { get; set; }
    }
}
