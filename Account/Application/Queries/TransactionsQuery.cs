using Binebase.Exchange.AccountService.Domain.Common;
using Binebase.Exchange.AccountService.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.AccountService.Application.Queries
{
    public class TransactionsQuery : IRequest<TransactionsQueryResult>,  IIdContainer
    {
        public Guid Id { get; set; }
        public Currency Currency { get; set; }
    }
}
