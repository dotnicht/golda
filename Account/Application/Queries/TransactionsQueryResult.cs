using Binebase.Exchange.AccountService.Application.Common.Mappings;
using Binebase.Exchange.AccountService.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.AccountService.Application.Queries
{
    public class TransactionsQueryResult
    {
        public Transaction[] Transactions { get; set; }

        public class Transaction : IIdContainer, IDateTimeContainer
        {
            public Guid Id { get; set; }
            public decimal Amount { get; set; }
            public decimal Balance { get; set; }
            public DateTime DateTime { get; set; }  
            public string Payload { get; set; }
        }
    }
}
