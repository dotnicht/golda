using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class BalanceQueryValidator : AbstractValidator<BalanceQuery>
    {
        public BalanceQueryValidator()
        {
            //RuleFor(x => x.Currency)
        }
    }
}
