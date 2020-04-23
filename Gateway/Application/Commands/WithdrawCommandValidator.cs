using FluentValidation;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class WithdrawCommandValidator : AbstractValidator<WithdrawCommand>
    {
        public WithdrawCommandValidator()
        {
            RuleFor(x => x.Address).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.Currency).IsInEnum();
        }
    }
}
