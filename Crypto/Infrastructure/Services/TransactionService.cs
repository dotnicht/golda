using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Binebase.Exchange.CryptoService.Domain.Entities;
using Microsoft.Extensions.Options;
using NBitcoin;
using QBitNinja.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Infrastructure.Services
{
    public class TransactionService : ITransactionService, IConfigurationProvider<TransactionService.Configuration>, ITransient<ITransactionService>
    {
        private readonly Configuration _configuration;

        public TransactionService(IOptions<Configuration> options)
            => _configuration = options.Value;

        public async Task<Domain.Entities.Transaction[]> GetTransactions(Currency currency, string address)
        {
            var network = _configuration.IsTestNet ? Network.TestNet : Network.Main;
            var client = new QBitNinjaClient(network);
            var summary = await client.GetBalanceSummary(BitcoinAddress.Create(address, network));
            throw new NotImplementedException();
        }

        public Task Subscribe()
        {
            throw new NotImplementedException();
        }

        public class Configuration
        {
            public bool IsTestNet { get; set; }
            public TimeSpan TransactionPoolingTimeout { get; set; }
        }
    }
}
