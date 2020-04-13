using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using System;

namespace Binebase.Exchange.Gateway.Infrastructure.Configuration
{

    public class Crypto : IConfig
    {
        public Uri Address { get; set; }
        public Currency[] Currencies { get; set; }
        public TimeSpan TransactionsSyncTimeout { get; set; }
    }
}
