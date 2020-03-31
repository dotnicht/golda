using FluentValidation;
using MediatR;
using System;

namespace Binebase.Exchange.AccountService.Contracts.Commands
{
    public class UnlockAccountCommand : IRequest
    {
        public Guid Id { get; set; }

        public class Valivator : AbstractValidator<UnlockAccountCommand>
        {
            public Valivator()
            {
                RuleFor(x => x.Id).NotEmpty();
            }
        }
    }
}
