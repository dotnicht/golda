using MediatR;
using System;

namespace Binebase.Exchange.AccountService.Contracts.Commands
{
    public class LockAccountCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
