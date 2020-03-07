using Binebase.Exchange.Common.Application.Interfaces;
using System;

namespace Binebase.Exchange.AccountService.Application.Commands
{
    public class CreditAccountCommandResult : IIdContainer
    {
        public Guid Id { get; set; }
    }
}
