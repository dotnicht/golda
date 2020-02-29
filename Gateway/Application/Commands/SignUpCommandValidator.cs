using FluentValidation;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class SignUpCommandValidator : AbstractValidator<SignUpCommand>
    {
        public SignUpCommandValidator()
        {
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Confirmation).Equal(x => x.Password);
        }
    }
}
