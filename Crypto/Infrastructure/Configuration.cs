using System;

namespace Binebase.Exchange.CryptoService.Infrastructure
{
    public class Configuration
    {
        public bool IsTestNet { get; set; }
        public string Mnemonic { get; set; }
        public string Password { get; set; }
        public int WithdrawAccountIndex { get; set; }
        public Uri AccountService { get; set; }
        public bool DebitDepositTransactions { get; set; }
        public TimeSpan TransactionPoolingTimeout { get; set; }
        public string EtherscanUrlFormat { get; set; }
        public Uri EthereumNode { get; set; }
    }
}
