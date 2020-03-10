using FluentValidation;

namespace Binebase.Exchange.AccountService.Application.Queries
{
    public class BalanceQueryValidator : AbstractValidator<BalanceQuery>
    {
        public BalanceQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
