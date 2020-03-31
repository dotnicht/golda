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
            public decimal Amount { get; set; }
            public decimal Balance { get; set; }
            public DateTime DateTime { get; set; }
            public string Payload { get; set; }
            public Currency Currency { get; set; }
        }
    }
}
