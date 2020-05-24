using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Domain.Enums;
using System;

namespace Binebase.Exchange.CryptoService.Domain.Entities
{
    public class Transaction : AuditableEntity
    {
        public TransactionDirection Direction { get; set; }
        public ulong? Block { get; set; }
        public string Hash { get; set; }
        public ulong RawAmount { get; set; }
        public decimal Amount { get; set; }
        public DateTime? Confirmed { get; set; }
        public int Confirmations { get; set; }
        public TransactionStatus Status { get; set; }
        public Guid AddressId { get; set; }
        public Address Address { get; set; }
    }
}
