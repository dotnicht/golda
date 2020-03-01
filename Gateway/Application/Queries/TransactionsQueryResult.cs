using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Enums;
using System;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class TransactionsQueryResult
    {
        public Transaction[] Transactions { get; set; }

        public class Transaction : IIdContainer
        {
            public Guid Id { get; set; }
            public DateTime DateTime { get; set; }
            public Currency Currency { get; set; }
            public decimal Amount { get; set; }
            public decimal Balance { get; set; }
            public TransactionSource Source { get; set; }
        }
    }
}
