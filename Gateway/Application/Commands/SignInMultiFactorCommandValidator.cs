using FluentValidation;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class MultyFactorSignInCommandValidator : AbstractValidator<SignInMultiFactorCommand>
    {
        public MultyFactorSignInCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty();
        }
    }
}
