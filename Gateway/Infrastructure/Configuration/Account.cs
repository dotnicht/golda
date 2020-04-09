using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using System;

namespace Binebase.Exchange.Gateway.Infrastructure.Configuration
{
    public class Account : IConfig
    {
        public Uri Address { get; set; }
        public Currency[] Currencies { get; set; }
        public TimeSpan PortfolioCacheExpiration { get; set; }
    }
}
