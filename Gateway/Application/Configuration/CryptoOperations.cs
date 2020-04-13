using Binebase.Exchange.Common.Application.Interfaces;

namespace Binebase.Exchange.Gateway.Application.Configuration
{
    public class CryptoOperations : IConfig
    {
        public decimal WithdrawDailyLimit { get; set; }
        public bool WithdrawMultiRequired { get; set; }
        public int WithdrawMiningRequirement { get; set; }
        public int ExchangeMiningRequirement { get; set; }
    }
}
