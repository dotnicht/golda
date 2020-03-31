using MediatR;
using System;

namespace Binebase.Exchange.AccountService.Contracts.Commands
{
    public class UnlockAccountCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
