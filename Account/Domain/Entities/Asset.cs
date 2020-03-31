using Binebase.Exchange.Common.Domain;
using System;

namespace Binebase.Exchange.AccountService.Domain.Entities
{
    public class Asset
    {
        public Guid Id { get; set; }
        public Currency Currency { get; set; }
        public decimal Balance { get; set; }
        public bool IsLocked { get; set; }
    }
}
