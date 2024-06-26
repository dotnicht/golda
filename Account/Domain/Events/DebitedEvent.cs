﻿using Binebase.Exchange.Common.Domain;
using System;

namespace Binebase.Exchange.AccountService.Domain.Events
{
    public class DebitedEvent
    {
        public Guid AssetId { get; set; }
        public Guid TransactionId { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
    }
}
