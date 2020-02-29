using Binebase.Exchange.Gateway.Application.Common.Mappings;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class MiningDailyCommandResult
    {
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
    }
}
