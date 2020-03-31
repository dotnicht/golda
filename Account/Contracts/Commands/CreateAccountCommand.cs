using MediatR;
using System;

namespace Binebase.Exchange.AccountService.Contracts.Commands
{
    public class CreateAccountCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
