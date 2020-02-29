using Binebase.Exchange.AccountService.Domain.Common;
using MediatR;
using System;

namespace Binebase.Exchange.AccountService.Application.Queries
{
    public class PortfolioQuery : IRequest<PortfolioQueryResult>, IIdContainer
    {
        public Guid Id { get; set; }
    }
}
