using FluentValidation;
using MediatR;
using System;

namespace Binebase.Exchange.AccountService.Contracts.Commands
{
    public class LockAccountCommand : IRequest
    {
        public Guid Id { get; set; }

        public class Valivator : AbstractValidator<LockAccountCommand>
        {
            public Valivator()
            {
                RuleFor(x => x.Id).NotEmpty();
            }
        }
    }
}
