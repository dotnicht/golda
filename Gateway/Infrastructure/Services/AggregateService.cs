using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Infrastructure.Services
{
    public class AggregateService : IAggregateService
    {
        private readonly IUserContext _userContext;
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IAccountService _accountService;

        public AggregateService(IUserContext userContext, IApplicationDbContext applicationDbContext, IAccountService accountService)
            => (_userContext, _applicationDbContext, _accountService) = (userContext, applicationDbContext, accountService);

        public async Task PopulateBalances()
        {
            var currencies = new[] { Currency.BINE, Currency.EURB, Currency.BTC, Currency.ETH };
            var transactions = currencies.ToDictionary(x => x, x => new List<Transaction>());
            var balances = currencies.ToDictionary(x => x, x => 0M);
            
            foreach (var id in _userContext.Users.Where(x => !x.IsSystem).Select(x => x.Id))
            {
                var txs = await _accountService.GetTransactions(id);
                foreach (var group in txs.GroupBy(x => x.Currency))
                {
                    transactions[group.Key].AddRange(group);
                }

                var portfolio = await _accountService.GetPorfolio(id);
                foreach (var asset in portfolio)
                {
                    balances[asset.Key] += asset.Value;
                }
            }


        }
    }
}
