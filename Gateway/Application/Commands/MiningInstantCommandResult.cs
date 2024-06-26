﻿using Binebase.Exchange.Common.Application.Mappings;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Domain.Entities;
using System;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class MiningInstantCommandResult : IMapFrom<MiningRequest>
    {
        public Guid Id { get; set; }
        public int Index { get; set; }
        public decimal Amount { get; set; }
        public PromotionItem[] Promotions { get; set; }

        public class PromotionItem : IMapFrom<Promotion>
        {
            public Guid Id { get; set; }
            public Currency Currency { get; set; }
            public decimal TokenAmount { get; set; }
            public decimal CurrencyAmount { get; set; }
        }
    }
}
