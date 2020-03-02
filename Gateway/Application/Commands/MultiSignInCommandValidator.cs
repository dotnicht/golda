using FluentValidation;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    class MultiSignInCommandValidator : AbstractValidator<MultySignInCommand>
    {
        public MultiSignInCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty();
           
        }
    }
}
