using FluentValidation;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class ExchangePromotionCommandValidator : AbstractValidator<ExchangePromotionCommand>
    {
        public ExchangePromotionCommandValidator()
        {
            RuleFor(x => x.Promotions).NotNull();
            RuleFor(x => x.Promotions).NotEmpty();
        }
    }
}
