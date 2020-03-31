using FluentValidation;
using MediatR;
using System;

namespace Binebase.Exchange.AccountService.Application.Queries
{
    public class BalanceQuery : IRequest<BalanceQueryResult>
    {
        public Guid Id { get; set; }
        public Guid AssetId { get; set; }

        public class BalanceQueryValidator : AbstractValidator<BalanceQuery>
        {
            public BalanceQueryValidator()
            {
                RuleFor(x => x.Id).NotEmpty();
                RuleFor(x => x.AssetId).NotEmpty();
            }
        }
    }
}
