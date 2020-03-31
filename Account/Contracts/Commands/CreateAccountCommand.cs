using MediatR;
using System;

namespace Binebase.Exchange.AccountService.Application.Commands
{
    public class CreateAccountCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
