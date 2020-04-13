using Binebase.Exchange.Common.Infrastructure.Clients.Account;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly AssetClient _accountClient;

        public AccountService(HttpClient httpClient, IOptions<Configuration> options)
            => _accountClient = new AssetClient(options.Value.AccountService.ToString(), httpClient);

        public async Task Debit(Guid accountId, Common.Domain.Currency currency, decimal amount, Guid externalId)
        {
            var portfolio = await _accountClient.PortfolioAsync(accountId);

            var cmd = new DebitCommand
            {
                Id = accountId,
                AssetId = portfolio.Portfolio.Single(x => x.Currency == (Currency)currency).Id,
                TransactionId = externalId,
                Amount = amount,
                Type = TransactionType.Deposit
            };

            await _accountClient.DebitAsync(cmd);
        }
    }
}
