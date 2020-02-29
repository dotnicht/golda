using FluentValidation;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class ConfirmCommandValidator : AbstractValidator<ConfirmCommand>
    {
        public ConfirmCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty();
        }
    }
}
