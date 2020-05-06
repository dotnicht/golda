using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Infrastructure.Configuration;
using Binebase.Exchange.Gateway.Infrastructure.Entities;
using Binebase.Exchange.Gateway.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Infrastructure.Services
{
    public class AggregateService : IAggregateService
    {
        private readonly Aggregation _configuration;
        private readonly ILogger _logger;
        private readonly IDateTime _dateTime;
        private readonly IUserContext _userContext;
        private readonly IAccountService _accountService;

        public AggregateService(
            IOptions<Aggregation> options,
            ILogger<AggregateService> logger,
            IDateTime dateTime,
            IUserContext userContext,
            IAccountService accountService)
            => (_configuration, _logger, _dateTime, _userContext, _accountService)
            = (options.Value, logger, dateTime, userContext, accountService);

        public async Task PopulateBalances(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var previous = _userContext.BalanceRecords.OrderByDescending(x => x.To).FirstOrDefault();
                var from = previous?.From ?? default;

                var currencies = new[] { Currency.BINE, Currency.EURB, Currency.BTC, Currency.ETH };
                var transactions = currencies.ToDictionary(x => x, x => new List<Transaction>());
                var balances = currencies.ToDictionary(x => x, x => 0M);

                foreach (var id in _userContext.Users.Select(x => x.Id))
                {
                    var txs = await _accountService.GetTransactions(id);
                    var portfolio = await _accountService.GetPorfolio(id, true);

                    if (txs.Length > 0 && (from == default || txs[0].DateTime < from))
                    {
                        from = txs[0].DateTime;
                    }

                    foreach (var group in txs.GroupBy(x => x.Currency))
                    {
                        transactions[group.Key].AddRange(group);
                    }

                    foreach (var asset in portfolio)
                    {
                        balances[asset.Key] += asset.Value;
                    }
                }

                foreach (var currency in currencies)
                {
                    var debit = transactions[currency].Where(x => x.Amount > 0);
                    var credit = transactions[currency].Where(x => x.Amount < 0);

                    var entity = new BalanceConsistencyRecord
                    {
                        From = from,
                        To = _dateTime.UtcNow,
                        Currency = currency,
                        TotalDebit = debit.Sum(x => x.Amount),
                        TotalCredit = credit.Sum(x => x.Amount),
                        DebitCount = debit.Count(),
                        CreditCount = credit.Count(),
                        StartBalance = previous?.EndBalance ?? 0,
                        EndBalance = balances[currency],
                    };

                    _userContext.BalanceRecords.Add(entity);
                    await _userContext.SaveChangesAsync(cancellationToken);

                    var balance = entity.StartBalance + entity.TotalDebit - entity.TotalCredit;

                    if (balance != entity.EndBalance)
                    {
                        _logger.LogError("Balance inconsistency detected starting {from} ending {to}. Balance {expected} not equal to {actual}.", entity.From, entity.To, balance, entity.EndBalance);
                    }
                }

                await Task.Delay(_configuration.Timeout);
            }
        }
    }
}
