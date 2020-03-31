using MediatR;
using System;

namespace Binebase.Exchange.AccountService.Contracts.Commands
{
    public class NewAccountCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
