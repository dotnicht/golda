using FluentValidation;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class DisableMultyFactorCommandValidator : AbstractValidator<DisableMultyFactorCommand>
    {
        public DisableMultyFactorCommandValidator()
        {
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Code).NotEmpty();
        }
    }
}
