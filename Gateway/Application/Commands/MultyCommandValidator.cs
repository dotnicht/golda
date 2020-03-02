using FluentValidation;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class MultyCommandValidator : AbstractValidator<MultyCommand>
    {
        public MultyCommandValidator()
        {
            RuleFor(x => x.Code).NotEmpty();
        }
    }
}
