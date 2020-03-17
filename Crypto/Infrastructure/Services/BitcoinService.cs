using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.CryptoService.Application.Interfaces;

namespace Binebase.Exchange.CryptoService.Infrastructure.Services
{
    public class BitcoinService : IBitcoinService, IConfigurationProvider<BitcoinService.Configuration>, ITransient<IBitcoinService>
    {
        public class Configuration
        {
        }
    }
}
