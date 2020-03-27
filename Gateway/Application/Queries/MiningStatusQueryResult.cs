using System;
using System.Collections.Generic;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class MiningStatusQueryResult
    {
        public TimeSpan BonusTimeout { get; set; }
        public TimeSpan InstantTimeout { get; set; }
        public int InstantMiningCount { get; set; }
        public Dictionary<int, int> InstantBoostMapping { get; set; }
    }
}
