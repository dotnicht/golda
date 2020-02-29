using Binebase.Exchange.AccountService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.AccountService.Domain.Common
{
    public interface ITransaction
    {
        public Currency Currency { get; set; }
        public decimal Amount { get; set; }
    }
}
