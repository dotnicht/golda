using Binebase.Exchange.Common.Domain;
using MediatR;
using System;

namespace Binebase.Exchange.AccountService.Application.Commands
{
    public class AddAssetCommand : IRequest
    {
        public Guid Id { get; set; }
        public Guid AssetId { get; set; }
        public Currency Currency { get; set; }
    }
}