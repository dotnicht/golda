using Binebase.Exchange.AccountService.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.AccountService.Application.Commands
{
    public class CreditAccountCommandResult : IIdContainer
    {
        public Guid Id { get; set; }
    }
}
