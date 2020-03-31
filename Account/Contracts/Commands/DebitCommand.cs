﻿using Binebase.Exchange.Common.Domain;
using MediatR;
using System;

namespace Binebase.Exchange.AccountService.Application.Commands
{
    public class DebitCommand : IRequest
    {
        public Guid Id { get; set; }
        public Guid AssetId { get; set; }
        public Guid TransactionId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
    }
}
