using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Common.Infrastructure.Interfaces;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Microsoft.Extensions.Options;
using NBitcoin;
using Nethereum.Hex.HexTypes;
using Newtonsoft.Json;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Infrastructure.Services
{
    public class BlockchainService : IBlockchainService, IHttpClientScoped<IBlockchainService>
    {
        private readonly Configuration _configuration;
        private readonly HttpClient _httpClient;

        public BlockchainService(IOptions<Configuration> options, HttpClient httpClient)
            => (_configuration, _httpClient) = (options.Value, httpClient);

        public async Task<ulong> CurrentIndex(Currency currency)
            => currency switch
            {
                Currency.BTC => await CurrentIndexBitcoin(),
                Currency.ETH => await CurrentIndexEthereum(),
                _ => throw new NotSupportedException(),
            };

        public Task<string> PublishTransaction(Currency currency, decimal amount, string address)
        {
            throw new NotImplementedException();
        }

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

        private class EtherscanBlockNumberResponse
        {
            public string Result { get; set; }
        }
    }
}
