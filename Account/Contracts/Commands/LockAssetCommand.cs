using FluentValidation;
using MediatR;
using System;

namespace Binebase.Exchange.AccountService.Contracts.Commands
{
    public class LockAssetCommand : IRequest
    {
        public Guid Id { get; set; }
        public Guid AssetId { get; set; }

        public class Valivator : AbstractValidator<LockAssetCommand>
        {
            public Valivator()
            {
                RuleFor(x => x.Id).NotEmpty();
                RuleFor(x => x.AssetId).NotEmpty();
            }
        }
    }
}
