using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Infrastructure.Services
{
    public class CryptoService : ICryptoService, IConfigurationProvider<CryptoService.Configuration>, ITransient<ICryptoService>
    {
        private static readonly Dictionary<Currency, string> _addresses = new Dictionary<Currency, string> 
        {
            [Currency.BTC] = "bc1q8g7qt8wkls2drqtvxtewdxxwzrtfm5ar44jqcu",
            [Currency.ETH] = "0x40947142eAB3C89fEbb3800E4F6fdc34c503A2C5"
        };

        public Task<string> GetAddress(Guid id, Currency currency)
        {
            return Task.FromResult(_addresses[currency]);
        }

        public Task<Dictionary<Currency, string>> GetAddresses(Guid id)
        {
            return Task.FromResult(_addresses);
        }

        public Task<string> GenerateAddress(Guid id, Currency currency)
        {
            return Task.FromResult(_addresses[currency]);
        }

        public class Configuration
        {
            public Uri Address { get; set; }
        }
    }
}
