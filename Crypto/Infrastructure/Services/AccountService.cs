using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Infrastructure.Clients.Account;
using Binebase.Exchange.Common.Infrastructure.Interfaces;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Infrastructure.Services
{
    public class AccountService : IAccountService, IHttpClientScoped<IAccountService>
    {
        private readonly AccountClient _accountClient;

        public AccountService(HttpClient httpClient, IOptions<Configuration> options)
            => _accountClient = new AccountClient(options.Value.AccountService.ToString(), httpClient);

        public async Task<Guid> Debit(Guid accountId, Common.Domain.Currency currency, decimal amount, Guid externalId)
        {
            var cmd = new DebitAccountCommand
            {
                Id = accountId,
                Currency = (Currency)currency,
                Amount = amount,
                Payload = JsonConvert.SerializeObject(new TransactionPayload { ExternalId = externalId })
            };

            return (await _accountClient.DebitAsync(cmd)).Id;
        }

        private class TransactionPayload
        {
            public Guid ExternalId { get; set; }
            public string Source => "Deposit"; // TODO: refactor after switching to common contracts.
        }
    }
}
