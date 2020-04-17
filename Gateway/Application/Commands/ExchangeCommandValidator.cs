using FluentValidation;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class ExchangeCommandValidator : AbstractValidator<ExchangeCommand>
    {
        public ExchangeCommandValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.Base).NotEqual(x => x.Quote);
        }
    }
}
