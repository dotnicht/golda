using FluentValidation;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class PhoneVerifyRequestCommandValidator : AbstractValidator<PhoneVerifyRequestCommand>
    {
        public PhoneVerifyRequestCommandValidator()
        {
            RuleFor(x => x.PhoneNumber).NotEmpty();
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
