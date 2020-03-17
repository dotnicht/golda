using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Common.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Binebase.Exchange.Gateway.Infrastructure.Services;
using Microsoft.Extensions.Options;
using NBitcoin;

namespace Binebase.Exchange.Gateway.Infrastructure.Crypto
{
    public class CryptoService : ICryptoService, IConfigurationProvider<CryptoService.Configuration>, ITransient<ICryptoService>
    {
        private readonly Configuration _configuration;
        private readonly CryptoClient _cryptoClient;

        public CryptoService(HttpClient client, IOptions<Configuration> options)
        {
            _configuration = options.Value;
            client.BaseAddress = _configuration.BaseUrl;
            _cryptoClient = new CryptoClient(_configuration.BaseUrl.ToString(), client);
        } 

        public async Task<string> GetAddress(Guid id, Currency currency)
        {
            return (await GetAddresses(id))[currency];
        }

        public async Task<Dictionary<Currency, string>> GetAddresses(Guid id)
        {
            var result = await _cryptoClient.Addresses2Async(id);
            return result.Addresses.ToDictionary(x => x.Currency, x => x.Public);
        }

        public async Task<string> GenerateAddress(Guid id, Currency currency)
        {
            var result = await _cryptoClient.AddressesAsync(new GenerateAddressCommand
            {
                Id = id,
                Currency = currency
            }); 
           return result.Address;
        }


        public class Configuration
        {
            public Uri BaseUrl { get; set; }
        }
    }
}
