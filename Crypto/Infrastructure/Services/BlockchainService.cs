using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Common.Infrastructure.Interfaces;
using Binebase.Exchange.CryptoService.Application;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Microsoft.Extensions.Options;
using NBitcoin;
using Nethereum.HdWallet;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Newtonsoft.Json;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Infrastructure.Services
{
    public class BlockchainService : IBlockchainService, IHttpClientScoped<IBlockchainService>
    {
        private readonly Configuration _configuration;
        private readonly HttpClient _httpClient;

        public BlockchainService(IOptions<Configuration> options, HttpClient httpClient, IApplicationDbContext context)
            => (_configuration, _httpClient) = (options.Value, httpClient);

        public async Task<ulong> CurrentIndex(Currency currency)
            => currency switch
            {
                Currency.BTC => await CurrentIndexBitcoin(),
                Currency.ETH => await CurrentIndexEthereum(),
                _ => throw new NotSupportedException(ErrorCode.CurrencyNotSupported),
            };

        public async Task<(string Hash, ulong Amount)> PublishTransaction(Currency currency, decimal amount, string address)
            => currency switch
            {
                Currency.BTC => await PublishTransactionBitcoin(amount, address),
                Currency.ETH => await PublishTransactionEthereum(amount, address),
                _ => throw new NotSupportedException(ErrorCode.CurrencyNotSupported),
            };

        private async Task<ulong> CurrentIndexBitcoin()
        {
            var client = new QBitNinjaClient(_configuration.IsTestNet ? Network.TestNet : Network.Main);
            var response = await client.GetBlock(new BlockFeature { Special = SpecialFeature.Last });
            return (ulong)response.Block.GetCoinbaseHeight().Value;
        }

        private async Task<ulong> CurrentIndexEthereum()
        {
            var response = await _httpClient.GetAsync(string.Format(_configuration.EtherscanUrlFormat, _configuration.IsTestNet ? "ropsten" : "api", "proxy", "eth_blockNumber"));
            var result = JsonConvert.DeserializeObject<EtherscanBlockNumberResponse>(await response.Content.ReadAsStringAsync());
            return new HexBigInteger(result.Result).ToUlong();
        }
        private async Task<(string Hash, ulong Amount)> PublishTransactionBitcoin(decimal amount, string address)
        {
            throw new NotImplementedException();
        }

        private async Task<(string Hash, ulong Amount)> PublishTransactionEthereum(decimal amount, string address)
        {
            var account = new Wallet(_configuration.Mnemonic, _configuration.Password).GetAccount(_configuration.WithdrawAccountIndex);
            var wei = Web3.Convert.ToWei(amount);
            var web3 = new Web3(account, _configuration.EthereumNode.ToString());
            web3.TransactionManager.DefaultGasPrice = await web3.Eth.GasPrice.SendRequestAsync();
            // TODO: check fee.
            //var fee = web3.TransactionManager.DefaultGas * web3.TransactionManager.DefaultGasPrice; 
            var hash = await web3.TransactionManager.SendTransactionAsync(account.Address, address, new HexBigInteger(wei));
            return (hash, (ulong)wei);
        }

        private class EtherscanBlockNumberResponse
        {
            public string Result { get; set; }
        }
    }
}
