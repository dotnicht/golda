using Binebase.Exchange.Common.Application.Interfaces;
using System;

namespace Binebase.Exchange.CryptoService.Infrastructure
{
    public class Configuration : IConfig
    {
        public bool IsTestNet { get; set; }
        public string Mnemonic { get; set; }
        public string Password { get; set; }
        public int WithdrawAccountIndex { get; set; }
        public int ConfirmationsCount { get; set; }
        public TimeSpan TransactionPoolingTimeout { get; set; }
        public string EtherscanUrlFormat { get; set; }
        public Uri EthereumNode { get; set; }
        public Uri EarnAddress { get; set; }
    }
}
