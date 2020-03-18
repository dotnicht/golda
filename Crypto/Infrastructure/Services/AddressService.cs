using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Microsoft.Extensions.Options;
using NBitcoin;
using Nethereum.HdWallet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Infrastructure.Services
{
    public class AddressService : IAddressService, IConfigurationProvider<AddressService.Configuration>, ITransient<IAddressService>
    {
        private readonly Configuration _configuration;

        public AddressService(IOptions<Configuration> options)
            => _configuration = options.Value;

        public async Task<string> GenerateAddress(Currency currency, int index)
            => currency switch
            {
                Currency.BTC => await GenerateBitcoinAddress(index),
                Currency.ETH => await GenerateEtherumAddress(index),
                _ => throw new NotSupportedException(),
            };

        private async Task<string> GenerateBitcoinAddress(int index)
        {
            var mnemo = new Mnemonic(_configuration.Mnemonic, Wordlist.English);
            var key = mnemo.DeriveExtKey(_configuration.Passwords[Currency.BTC]);
            var address = key.Derive((uint)index).ScriptPubKey.GetDestinationAddress(Network.Main);
            return await Task.FromResult(address.ToString());
        } 

        private async Task<string> GenerateEtherumAddress(int index)
        {
            var wallet = new Wallet(_configuration.Mnemonic, _configuration.Passwords[Currency.ETH]);
            return await Task.FromResult(wallet.GetAccount(index).Address);
        }

        public class Configuration
        {
            public string Mnemonic { get; set; }
            public Dictionary<Currency, string> Passwords { get; set; }
        }
    }
}
