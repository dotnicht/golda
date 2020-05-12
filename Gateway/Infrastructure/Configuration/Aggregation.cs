using Binebase.Exchange.Common.Application.Interfaces;
using System;

namespace Binebase.Exchange.Gateway.Infrastructure.Configuration
{
    public class Aggregation : IConfig
    {
        public TimeSpan Timeout { get; set; }
    }
}
