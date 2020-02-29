using FluentValidation;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class ExchangeRateQueryValidator : AbstractValidator<ExchangeRateQuery>
    {
        public ExchangeRateQueryValidator() 
            => RuleFor(x => x.Base).NotEqual(x => x.Quote).WithMessage("Base currency must not be equal to Quote currency.");
    }
}
