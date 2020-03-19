using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Domain.Enums;
using System;
using System.Numerics;

namespace Binebase.Exchange.CryptoService.Domain.Entities
{
    public class Transaction : AuditableEntity
    {
        public TransactionDirection Direction { get; set; }
        public long Block { get; set; }
        public string Hash { get; set; }
        public BigInteger Amount { get; set; }
        public DateTime Confirmed { get; set; }
        public Guid AddressId { get; set; }
        public Address Address { get; set; }
    }
}
