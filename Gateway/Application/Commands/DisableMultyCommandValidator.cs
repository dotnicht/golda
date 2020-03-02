using FluentValidation;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class DisableMultyCommandValidator : AbstractValidator<DisableMultyCommand>
    {
        public DisableMultyCommandValidator()
        {
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Code).NotEmpty();
        }
    }
}
