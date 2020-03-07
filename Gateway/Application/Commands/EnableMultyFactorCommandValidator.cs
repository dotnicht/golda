using FluentValidation;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class EnableMultyFactorCommandValidator : AbstractValidator<EnableMultyFactorCommand>
    {
        public EnableMultyFactorCommandValidator()
        {
            RuleFor(x => x.Code).NotEmpty();
        }
    }
}
