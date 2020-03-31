using MediatR;
using System;

namespace Binebase.Exchange.AccountService.Contracts.Commands
{
    public class RemoveAssetCommand : IRequest
    {
        public Guid Id { get; set; }
        public Guid AssetId { get; set; }
    }
}
