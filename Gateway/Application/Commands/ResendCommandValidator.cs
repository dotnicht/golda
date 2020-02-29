using FluentValidation;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class ResendCommandValidator : AbstractValidator<ResendCommand>
    {
        public ResendCommandValidator()
        {
            RuleFor(x => x.Email).EmailAddress();
        }
    }
}
