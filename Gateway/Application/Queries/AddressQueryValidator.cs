using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class AddressQueryValidator : AbstractValidator<AddressQuery>
    {
        public AddressQueryValidator()
        {
            //RuleFor(x => x.Currency)
        }
    }
}
