using Binebase.Exchange.AccountService.Domain.Events;
using Binebase.Exchange.Common.Application.Mappings;
using Binebase.Exchange.Common.Domain;
using System;

namespace Binebase.Exchange.AccountService.Application.Queries
{
    public class TransactionsQueryResult
    {
        public Transaction[] Transactions { get; set; }

        public class Transaction : IMapFrom<CreditedEvent>, IMapFrom<DebitedEvent>
        {
            public Guid Id { get; set; }
            public Guid AssetId { get; set; }
            public TransactionType Type { get; set; }
            public DateTime DateTime { get; set; }
            public decimal Amount { get; set; }
        }
    }
}
