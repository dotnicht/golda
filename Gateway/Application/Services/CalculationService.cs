using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.Enums;
using Binebase.Exchange.Gateway.Domain.ValueObjects;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace Binebase.Exchange.Gateway.Application.Services
{
    public class CalculationService : ICalculationService, IConfigurationProvider<CalculationService.Configuration>, ITransient<ICalculationService>
    {
        private readonly IAccountService _accountService;
        private readonly IExchangeRateService _exchangeRateService;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;
        private readonly Configuration _configuration;

        public CalculationService(
            IAccountService accountService,
            IExchangeRateService exchangeRateService,
            IIdentityService identityService,
            ICurrentUserService currentUserService,
            IDateTime dateTime,
            IOptions<Configuration> options)
            => (_accountService, _exchangeRateService, _identityService, _currentUserService, _dateTime, _configuration)
                = (accountService, exchangeRateService, identityService, currentUserService, dateTime, options.Value);

        public Task<decimal> GenerateDefaultReward()
            => Task.FromResult(RandomInRange(_configuration.DefaultRange[0], _configuration.DefaultRange[1]));

        public async Task<(decimal Amount, TransactionType Type)> GenerateWeeklyMiningReward()
        {
            var txs = (await _accountService.GetTransactions(_currentUserService.UserId)).Where(x => x.Currency == Currency.BINE).OrderByDescending(x => x.DateTime);
            var bonus = txs.FirstOrDefault(x => x.Type == TransactionType.Bonus);

            if (bonus != null && bonus.DateTime < _dateTime.UtcNow - _configuration.Weekly.Timeout)
            {
                throw new InvalidOperationException($"Weekly timeout is active.");
            }

            var balance = await GetBalance();
            var amount = await GenerateDefaultReward();
            var type = TransactionType.Default;

            if (balance > _configuration.BalanceTreshold)
            {
                type = TransactionType.Bonus;

                if (new Random().NextDouble() > _configuration.Weekly.Probability)
                {
                    var user = await _identityService.GetUser(_currentUserService.UserId);
                    amount = balance * _configuration.Weekly.Coefficients[(int)Math.Floor((_dateTime.UtcNow - user.Registered).TotalDays / 7)];
                }
            }

            return (amount, type);
        }

        public async Task<(decimal Amount, TransactionType Type)> GenerateBonusMiningReward()
        {
            var amount = 0M;
            var type = TransactionType.Default;

            var txs = (await _accountService.GetTransactions(_currentUserService.UserId)).Where(x => x.Currency == Currency.BINE).OrderByDescending(x => x.DateTime);

            var target = txs
                .Where(x => x.Source == TransactionSource.Mining && x.Type == TransactionType.Weekly)
                .Take(_configuration.Bonus.StackTimes)
                .Select((x, i) => new { Index = i, Target = x })
                .ToDictionary(x => x.Index, x => x.Target);

            var bonus = txs.FirstOrDefault(x => x.Type == TransactionType.Bonus);

            if (target.Count >= _configuration.Bonus.StackTimes
                && target.All(x => x.Key == 0 || x.Value.DateTime - target[x.Key - 1].DateTime <= _configuration.Bonus.Window * 2)
                && (bonus == null || bonus.DateTime < _dateTime.UtcNow - _configuration.Bonus.Timeout)
                && new Random().NextDouble() > _configuration.Bonus.Probability)
            {
                type = TransactionType.Bonus;
                var part = RandomInRange(_configuration.Bonus.Range[0], _configuration.Bonus.Range[1]);
                amount = part * await GetBalance();
            }

            return (amount, type);
        }

        public async Task<decimal> GenerateInstantMiningReward()
        {
            var bine = 0M;
            if (new Random().NextDouble() < _configuration.Instant.Probability)
            {
                var rate = await _exchangeRateService.GetExchangeRate(new Pair(Currency.BINE, Currency.EURB));
                bine = _configuration.Instant.Fee / rate.Rate;
                var rnd = new Random().NextDouble();

                foreach (var range in _configuration.Instant.Categories.OrderBy(x => x.Value))
                {
                    if (rnd <= range.Value)
                    {
                        bine *= range.Key switch
                        {
                            Configuration.InstantItem.Category.x2 => 2,
                            Configuration.InstantItem.Category.x2x5 => RandomInRange(2, 5),
                            Configuration.InstantItem.Category.x5x10 => RandomInRange(5, 10),
                            Configuration.InstantItem.Category.x10x100 => RandomInRange(10, 100),
                            _ => throw new NotSupportedException(),
                        };
                        break;
                    }
                }
            }

            return bine;
        }

        public async Task<Promotion> GeneratePromotion(decimal reward)
        {
            var promotion = null as Promotion;

            if (reward > 0 && new Random().NextDouble() < _configuration.Promotion.Probability)
            {
                promotion = new Promotion { Id = Guid.NewGuid() };
                // TODO: promotion.
            }

            return await Task.FromResult(promotion);
        }

        public async Task<TimeSpan> GetWeeklyTimeout()
            => await GetTimeout(TransactionType.Weekly, _configuration.Weekly.Timeout);

        public async Task<TimeSpan> GetInstantTimeout()
            => await GetTimeout(TransactionType.Instant, _configuration.Instant.Timeout);

        public async Task<int> GetCurrentMiningCount()
        {
            var txs = (await _accountService.GetTransactions(_currentUserService.UserId)).Where(x => x.Currency == Currency.BINE);
            return txs.Count(x => x.Source == TransactionSource.Mining && x.Type == TransactionType.Instant);
        }

        public Task<Dictionary<int, int>> GetInstantBoostMapping()
            => Task.FromResult(_configuration.Instant.BoostMapping);

        public Task<decimal> GetInstantMiningFee()
            => Task.FromResult(_configuration.Instant.Fee);

        public Task<TimeSpan> GetMiningRequestWindow()
            => Task.FromResult(_configuration.MiningRequestWindow);

        private async Task<TimeSpan> GetTimeout(TransactionType type, TimeSpan timeout)
        {
            var txs = (await _accountService.GetTransactions(_currentUserService.UserId)).Where(x => x.Currency == Currency.BINE);
            var last = txs.OrderByDescending(x => x.DateTime).FirstOrDefault(x => x.Type == type && x.Source == TransactionSource.Mining);
            return last == null || last.DateTime <= _dateTime.UtcNow - timeout ? default : _dateTime.UtcNow - last.DateTime;
        }

        private async Task<decimal> GetBalance()
        {
            var bine = await _accountService.GetBalance(_currentUserService.UserId, Currency.BINE);
            var eurb = await _accountService.GetBalance(_currentUserService.UserId, Currency.EURB);

            var ex = await _exchangeRateService.GetExchangeRate(new Pair(Currency.BINE, Currency.EURB));
            var balance = eurb * (1 / ex.Rate) + bine;
            return balance;
        }

        private decimal RandomInRange(decimal start, decimal end)
            => (decimal)new Random().NextDouble() * (end - start) + start;

        public class Configuration
        {
            public TimeSpan MiningRequestWindow { get; set; }
            public decimal[] DefaultRange { get; set; }
            public decimal BalanceTreshold { get; set; }
            public WeeklyItem Weekly { get; set; }
            public BonusItem Bonus { get; set; }
            public InstantItem Instant { get; set; }
            public PromotionItem Promotion { get; set; }

            public class WeeklyItem
            {
                public TimeSpan Timeout { get; set; }
                public decimal[] Coefficients { get; set; }
                public double Probability { get; set; }
            }

            public class BonusItem
            {
                public TimeSpan Timeout { get; set; }
                public TimeSpan Window { get; set; }
                public int StackTimes { get; set; }
                public double Probability { get;  set; }
                public decimal[] Range { get; set; }
            }

            public class InstantItem
            {
                public TimeSpan Timeout { get; set; }
                public Dictionary<int, int> BoostMapping { get; set; }
                public double Probability { get; set; }
                public decimal Fee { get; set; }
                public Dictionary<Category, double> Categories { get; set; }

                public enum Category
                {
                    x2, x2x5, x5x10, x10x100
                }
            }

            public class PromotionItem
            {
                public double Probability { get; set; }
                public decimal[] ExchangeRange { get; set; }
                public Dictionary<Currency, double[]> Currencies { get; set; }
            }
        }
    }
}
