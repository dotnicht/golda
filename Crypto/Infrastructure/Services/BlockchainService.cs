using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Microsoft.Extensions.Options;
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
        {
            return await Task.FromResult(0UL); // TODO: get current block index.
        }

        public Task<string> PublishTransaction(Currency currency, decimal amount, string address)
        {
            throw new NotImplementedException();
        }

        public class Configuration
        {
            public bool IsTestNet { get; set; }
        }
    }
}
