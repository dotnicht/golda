using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.AccountService.Application.Queries
{
    public class PortfolioQueryValidator : AbstractValidator<PortfolioQuery>
    {
        public PortfolioQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
