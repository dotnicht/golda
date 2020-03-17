using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Infrastructure.Crypto
{
    public class CryptoService : ICryptoService, IConfigurationProvider<CryptoService.Configuration>, ITransient<ICryptoService>
    {
        private readonly Configuration _configuration;
        private readonly CryptoClient _cryptoClient;

        public CryptoService(HttpClient client, IOptions<Configuration> options)
        {
            _configuration = options.Value;
            client.BaseAddress = _configuration.Address;
            _cryptoClient = new CryptoClient(_configuration.Address.ToString(), client);
        }

        public async Task<string> GetAddress(Guid id, Common.Domain.Currency currency)
        {
            return (await GetAddresses(id))[currency];
        }

        public async Task<Dictionary<Common.Domain.Currency, string>> GetAddresses(Guid id)
        {
            var result = await _cryptoClient.Addresses2Async(id);
            return result.Addresses.ToDictionary(x => (Common.Domain.Currency)x.Currency, x => x.Public);
        }

        public async Task<string> GenerateAddress(Guid id, Common.Domain.Currency currency)
        {
            var result = await _cryptoClient.AddressesAsync(new GenerateAddressCommand
            {
                Id = id,
                Currency = (Currency)currency
            });

            return result.Address;
        }

        public class Configuration
        {
            public Uri Address { get; set; }
        }
    }
}
