using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Infrastructure.Clients.Account;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Infrastructure.Services
{
    public class AccountService : IAccountService, IConfigurationProvider<AccountService.Configuration>, ITransient<IAccountService>
    {
        private readonly AccountClient _accountClient;

        public AccountService(HttpClient httpClient, IOptions<Configuration> options)
            => _accountClient = new AccountClient(options.Value.Address.ToString(), httpClient);

        public async Task<Guid> Debit(Guid accountId, Common.Domain.Currency currency, decimal amount, Guid externalId)
        {
            var cmd = new DebitAccountCommand
            {
                Id = accountId,
                Currency = (Common.Infrastructure.Clients.Account.Currency)currency,
                Amount = amount,
                Payload = JsonConvert.SerializeObject(new TransactionPayload { ExternalId = externalId })
            };

            return (await _accountClient.DebitAsync(cmd)).Id;
        }

        public Task<Guid> Credit(Guid accountId, Common.Domain.Currency currency, decimal amount, Guid externalId)
        {
            throw new NotImplementedException();
        }

        public class Configuration
        {
            public Uri Address { get; set; }
        }

        private class TransactionPayload
        {
            public Guid ExternalId { get; set; }
            public string Type => "Deposit";
        }
    }
}
