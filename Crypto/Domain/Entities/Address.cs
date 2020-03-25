using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Binebase.Exchange.CryptoService.Domain.Entities
{
    public class Address : AuditableEntity
    {
        public Guid AccountId { get; set; }
        public Currency Currency { get; set; }
        public AddressType Type { get; set; }
        public string Public { get; set; }
        public BigInteger Balance { get; set; }
        public long GeneratedBlock { get; set; }
        public long LastBlock { get; set; }
        public virtual ICollection<Transaction> Transactions { get; } = new HashSet<Transaction>();
    }
}
