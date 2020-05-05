using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            foreach (var id in _userContext.Users.Where(x => !x.IsSystem).Select(x => x.Id))
            {
                var txs = await _accountService.GetTransactions(id);
                var deposit = txs.Where(x => x.Type == TransactionType.Deposit).ToArray();
            }
        }
    }
}
