using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Domain.Enums;
using System;

namespace Admin.Models
{
    public class TransactionExt
    {
        private Binebase.Exchange.Gateway.Domain.Entities.Transaction _transaction;
        public TransactionExt(Binebase.Exchange.Gateway.Domain.Entities.Transaction transaction)
        {
            _transaction = transaction;
        }
        public TransactionExt()
        {
        }

        public Guid UserId { get; set; }
        public Currency Currency { get; set; } //Currency, Amount,Balance,DateTime,Source,,Type,CreatedBy,LastModifiedBy,Id,Created,LastModified
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public DateTime DateTime { get; set; }
        public TransactionType Type { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? LastModifiedBy { get; set; }
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
