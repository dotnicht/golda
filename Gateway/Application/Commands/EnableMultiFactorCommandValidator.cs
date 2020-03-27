using FluentValidation;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class EnableMultiFactorCommandValidator : AbstractValidator<EnableMultiFactorCommand>
    {
        public EnableMultiFactorCommandValidator()
        {
            RuleFor(x => x.Code).NotEmpty();
        }
    }
}
