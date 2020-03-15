using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Microsoft.Extensions.Options;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Infrastructure.Services
{
    public class AddressService : IAddressService, IConfigurationProvider<AddressService.Configuration>, ITransient<IAddressService>
    {
        private readonly Configuration _configuration;

        public AddressService(IOptions<Configuration> options)
            => _configuration = options.Value;

        public Task<string> GenerateAddress(Currency currency)
        {


            throw new NotImplementedException();
        }

        public class Configuration
        {
            public string Mnemonic { get; set; }
            public Dictionary<Currency, string> Passwords { get; set; }
        }
    }
}
