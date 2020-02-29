﻿using Binebase.Exchange.Gateway.Application.Common.Mappings;
using Binebase.Exchange.Gateway.Domain.Entities;
using System;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class MiningRequestCommandResult : IMapFrom<MiningRequest>
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
    }
}
