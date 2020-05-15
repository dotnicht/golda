using FluentValidation;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class VerifyPhoneNumberCommandValidator : AbstractValidator<VerifyPhoneNumberCommand>
    {
        public VerifyPhoneNumberCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty();
            RuleFor(x => x.PhoneNumber).NotEmpty();
        }
    }
}
