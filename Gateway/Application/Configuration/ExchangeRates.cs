using Binebase.Exchange.Common.Application.Interfaces;
using System;

namespace Binebase.Exchange.Gateway.Application.Configuration
{
    public class ExchangeRates : IConfig
    {
        public string[] SupportedPairs { get; set; }
        public string[] ExchangeExcludePairs { get; set; }
        public decimal BineBaseValue { get; set; }
        public decimal[] BineRange { get; set; }
        public TimeSpan BineRefreshRate { get; set; }
        public decimal ExchangeFee { get; set; }
    }
}
