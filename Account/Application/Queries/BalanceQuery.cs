using Binebase.Exchange.AccountService.Domain.Common;
using Binebase.Exchange.AccountService.Domain.Enums;
using MediatR;
using System;

namespace Binebase.Exchange.AccountService.Application.Queries
{
    public class BalanceQuery : IRequest<BalanceQueryResult>, IIdContainer
    {
        public Guid Id { get; set; }
        public Currency Currency { get; set; }
    }
}
