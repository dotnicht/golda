using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Microsoft.Extensions.Options;
using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Infrastructure.Services
{
    public class BlockchainService : IBlockchainService, IConfigurationProvider<BlockchainService.Configuration>, ITransient<IBlockchainService>
    {
        private readonly Configuration _configuration;

        public BlockchainService(IOptions<Configuration> options)
            => _configuration = options.Value;

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
            return await Task.FromResult(0UL);
        }

        public class Configuration
        {
            public bool IsTestNet { get; set; }
        }
    }
}
