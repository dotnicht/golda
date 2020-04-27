using FluentValidation;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class ExchangeCommandValidator : AbstractValidator<ExchangeCommand>
    {
        public ExchangeCommandValidator()
        {
            RuleFor(x => x.BaseAmount).NotEqual(x => x.QuoteAmount);
            RuleFor(x => x.Base).NotEqual(x => x.Quote);
        }
    }
}
