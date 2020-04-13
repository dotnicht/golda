using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using System;
using System.Collections.Generic;

namespace Binebase.Exchange.Gateway.Application.Configuration
{
    public class MiningCalculation : IConfig
    {
        public TimeSpan MiningRequestWindow { get; set; }
        public decimal[] DefaultRange { get; set; }
        public decimal BalanceTreshold { get; set; }
        public WeeklyItem Weekly { get; set; }
        public BonusItem Bonus { get; set; }
        public InstantItem Instant { get; set; }
        public PromotionItem Promotion { get; set; }

        public class WeeklyItem
        {
            public TimeSpan Timeout { get; set; }
            public decimal[] Coefficients { get; set; }
            public decimal Probability { get; set; }
        }

        public class BonusItem
        {
            public TimeSpan Timeout { get; set; }
            public TimeSpan Window { get; set; }
            public int StackTimes { get; set; }
            public decimal Probability { get; set; }
            public decimal[] Range { get; set; }
        }

        public class InstantItem
        {
            public TimeSpan Timeout { get; set; }
            public Dictionary<string, int> BoostMapping { get; set; }
            public decimal Probability { get; set; }
            public decimal Fee { get; set; }
            public Dictionary<Category, decimal> Categories { get; set; }

            public enum Category
            {
                x2,
                x2x5,
                x5x10,
                x10x100
            }
        }

        public class PromotionItem
        {
            public decimal Probability { get; set; }
            public Dictionary<Currency, decimal> Currencies { get; set; }
            public Dictionary<Category, decimal> Categories { get; set; }

            public enum Category
            {
                LastRange,
                LastAll,
                AllRange
            }
        }
    }
}
