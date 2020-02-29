using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.AccountService.Application.Queries
{
    public class BalanceQueryValidator : AbstractValidator<BalanceQuery>
    {
        public BalanceQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Currency).NotEmpty();
        }
    }
}
