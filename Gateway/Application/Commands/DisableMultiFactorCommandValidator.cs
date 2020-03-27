using FluentValidation;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class DisableMultiFactorCommandValidator : AbstractValidator<DisableMultiFactorCommand>
    {
        public DisableMultiFactorCommandValidator()
        {
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Code).NotEmpty();
        }
    }
}
