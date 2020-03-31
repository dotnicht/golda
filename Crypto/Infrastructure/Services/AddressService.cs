using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Microsoft.Extensions.Options;
using NBitcoin;
using Nethereum.HdWallet;
using Nethereum.Util;
using System;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Infrastructure.Services
{
    public class AddressService : IAddressService, ITransient<IAddressService>
    {
        private readonly Configuration _configuration;

        private Network BitcoinNetwork => _configuration.IsTestNet ? Network.TestNet : Network.Main;

        public AddressService(IOptions<Configuration> options)
            => _configuration = options.Value;

        public async Task<string> GenerateAddress(Currency currency, uint index)
            => currency switch
            {
                Currency.BTC => await GenerateBitcoinAddress(index),
                Currency.ETH => new Wallet(_configuration.Mnemonic, _configuration.Password).GetAccount((int)index).Address,
                _ => throw new NotSupportedException(),
            };

        public async Task<bool> ValidateAddress(Currency currency, string address)
            => currency switch
            {
                Currency.BTC => await ValidateBitcoinAddress(address),
                Currency.ETH => AddressUtil.Current.IsValidEthereumAddressHexFormat(address),
                _ => throw new NotSupportedException(),
            };

        private async Task<string> GenerateBitcoinAddress(uint index)
        {
            var mnemo = new Mnemonic(_configuration.Mnemonic, Wordlist.English);
            var key = mnemo.DeriveExtKey(_configuration.Password);
            var address = key.Derive(index).ScriptPubKey.GetDestinationAddress(BitcoinNetwork);
            return await Task.FromResult(address.ToString());
        } 

        private async Task<bool> ValidateBitcoinAddress(string address)
        {
            try
            {
                BitcoinAddress.Create(address, BitcoinNetwork);
                return await Task.FromResult(true);
            }
            catch
            {
                return false;
            }
        }
    }
}
