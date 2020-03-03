using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class ExchangeCommandValidator : AbstractValidator<ExchangeCommand>
    {
        public ExchangeCommandValidator()
        {
            RuleFor(x => x.Amount).NotEmpty();
        }
    }
}
