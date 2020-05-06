using FluentValidation;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class VerifyPhoneNumberCommandValidator : AbstractValidator<VerifyPhoneNumberCommand>
    {
        public VerifyPhoneNumberCommandValidator()
        {
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty();
        }

    }
}
