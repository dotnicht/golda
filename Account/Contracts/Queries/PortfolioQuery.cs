using FluentValidation;
using MediatR;
using System;

namespace Binebase.Exchange.AccountService.Application.Queries
{
    public class PortfolioQuery : IRequest<PortfolioQueryResult>
    {
        public Guid Id { get; set; }

        public class PortfolioQueryValidator : AbstractValidator<PortfolioQuery>
        {
            public PortfolioQueryValidator()
            {
                RuleFor(x => x.Id).NotEmpty();
            }
        }
    }
}
