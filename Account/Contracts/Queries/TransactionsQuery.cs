using FluentValidation;
using MediatR;
using System;

namespace Binebase.Exchange.AccountService.Application.Queries
{
    public class TransactionsQuery : IRequest<TransactionsQueryResult>
    {
        public Guid Id { get; set; }

        public class TransactionsQueryValidator : AbstractValidator<TransactionsQuery>
        {
            public TransactionsQueryValidator()
            {
                RuleFor(x => x.Id).NotEmpty();
            }
        }
    }
}
