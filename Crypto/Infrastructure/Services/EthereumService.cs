using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Application;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Binebase.Exchange.CryptoService.Domain.Entities;
using Binebase.Exchange.CryptoService.Domain.Enums;
using Microsoft.Extensions.Options;
using Nethereum.HdWallet;
using Nethereum.Hex.HexTypes;
using Nethereum.Util;
using Nethereum.Web3;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Infrastructure.Services
{
    public class EthereumService : IBlockchainService
    {
        private readonly Configuration _configuration;
        private readonly HttpClient _httpClient;

        public EthereumService(IOptions<Configuration> options, HttpClient httpClient) => (_configuration, _httpClient) = (options.Value, httpClient);

        public Currency Currency => Currency.ETH;

        public async Task<ulong> CurrentIndex()
            => (await new Web3(_configuration.EthereumNode.ToString()).Eth.Blocks.GetBlockNumber.SendRequestAsync()).ToUlong();

        public Task<string> GenerateAddress(uint index)
            => Task.FromResult(new Wallet(_configuration.Mnemonic, _configuration.Password).GetAccount((int)index).Address);

        public async Task<decimal> GetBalance(string address)
            => Web3.Convert.FromWei(await new Web3(_configuration.EthereumNode.ToString()).Eth.GetBalance.SendRequestAsync(address));

        public async Task<Transaction[]> GetTransactions(string address)
        {
            var result = new List<Transaction>();

            foreach (var operation in new[] { "txlist", "txlistinternal" })
            {
                var uri = string.Format(_configuration.EtherscanUrlFormat,
                    _configuration.IsTestNet ? "ropsten" : "api",
                    "account",
                    $"{operation}&address={address}");

                var response = await _httpClient.GetAsync(uri);
                var content = await response.Content.ReadAsStringAsync();

                var tx = JsonConvert.DeserializeObject<EtherscanTransactionsResponse>(content).Result
                    .Select(x => new Transaction
                    {
                        Direction = TransactionDirection.Inbound,
                        Confirmed = DateTimeOffset.FromUnixTimeSeconds(x.TimeStamp).UtcDateTime,
                        Hash = x.Hash,
                        Block = x.BlockNumber,
                        RawAmount = x.Value,
                        Amount = Web3.Convert.FromWei(x.Value)
                    });

                result.AddRange(tx);
            }

            return result.ToArray();
        }

        public async Task<(string Hash, ulong Amount)> PublishTransaction(decimal amount, string address)
        {
            var account = new Wallet(_configuration.Mnemonic, _configuration.Password).GetAccount(_configuration.WithdrawAccountIndex);
            var web3 = new Web3(account, _configuration.EthereumNode.ToString());
            web3.TransactionManager.DefaultGasPrice = await web3.Eth.GasPrice.SendRequestAsync();

            var wei = new HexBigInteger(Web3.Convert.ToWei(amount));
            var balance = await web3.Eth.GetBalance.SendRequestAsync(account.Address);
            if (balance.Value < wei.Value)
            {
                throw new InvalidOperationException(ErrorCode.InsufficientBalance);
            }

            return (await web3.TransactionManager.SendTransactionAsync(account.Address, address, wei), wei.ToUlong());
        }

        public Task<bool> ValidateAddress(string address)
            => Task.FromResult(AddressUtil.Current.IsValidEthereumAddressHexFormat(address));

        private class EtherscanTransactionsResponse
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public Transaction[] Result { get; set; }

            public class Transaction
            {
                public ulong BlockNumber { get; set; }
                public long TimeStamp { get; set; }
                public string Hash { get; set; }
                public string BlockHash { get; set; }
                public uint TransactionIndex { get; set; }
                public string From { get; set; }
                public string To { get; set; }
                public ulong Value { get; set; }
                public ulong Confirmations { get; set; }
            }
        }
    }
}
