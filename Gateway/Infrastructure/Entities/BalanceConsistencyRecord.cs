using Binebase.Exchange.Common.Domain;
using System;

namespace Binebase.Exchange.Gateway.Infrastructure.Entities
{
    public class BalanceConsistencyRecord : AuditableEntity
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public Currency Currency { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public int DebitCount { get; set; }
        public int CreditCount { get; set; }
        public decimal StartBalance { get; set; }
        public decimal EndBalance { get; set; }
    }
}
