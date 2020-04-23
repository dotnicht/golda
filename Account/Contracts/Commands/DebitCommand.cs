using Binebase.Exchange.Common.Domain;
using FluentValidation;
using MediatR;
using System;

namespace Binebase.Exchange.AccountService.Contracts.Commands
{
    public class DebitCommand : IRequest
    {
        public Guid Id { get; set; }
        public Guid AssetId { get; set; }
        public Guid TransactionId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }

        public class Valivator : AbstractValidator<DebitCommand>
        {
            public Valivator()
            {
                RuleFor(x => x.Id).NotEmpty();
                RuleFor(x => x.AssetId).NotEmpty();
                RuleFor(x => x.TransactionId).NotEmpty();
                RuleFor(x => x.Amount).GreaterThan(0);
            }
        }
    }
}
