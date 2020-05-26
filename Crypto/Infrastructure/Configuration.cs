using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using System;
using System.Collections.Generic;

namespace Binebase.Exchange.CryptoService.Infrastructure
{
    public class Configuration : IConfig
    {
        public bool IsTestNet { get; set; }
        public string Mnemonic { get; set; }
        public string Password { get; set; }
        public int WithdrawAccountIndex { get; set; }
        public ulong ConfirmationsCount { get; set; }
        public TimeSpan TransactionPoolingTimeout { get; set; }
        public string EtherscanApiKey { get; set; }
        public Uri EthereumNode { get; set; }
        public Uri EarnAddress { get; set; }
        public Dictionary<Currency, string> TransferAddresses { get; set; }
    }
}
