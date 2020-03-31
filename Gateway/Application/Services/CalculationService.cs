using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.Enums;
using Binebase.Exchange.Gateway.Domain.ValueObjects;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Services
{
    public class CalculationService : ICalculationService, IConfigurationProvider<CalculationService.Configuration>, ITransient<ICalculationService>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAccountService _accountService;
        private readonly IExchangeRateService _exchangeRateService;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;
        private readonly Configuration _configuration;

        public TimeSpan MiningRequestWindow => _configuration.MiningRequestWindow;
        public TimeSpan WeeklyTimeout => _configuration.Weekly.Timeout;
        public TimeSpan InstantTimeout => _configuration.Instant.Timeout;
        public decimal InstantMiningFee => _configuration.Instant.Fee;
        public Dictionary<int, int> InstantBoostMapping => _configuration.Instant.BoostMapping.ToDictionary(x => int.Parse(x.Key), x => x.Value);
        public int OperationLockMiningCount => _configuration.Instant.OperationLockMiningCount;

        public CalculationService(
            IApplicationDbContext context,
            IAccountService accountService,
            IExchangeRateService exchangeRateService,
            IIdentityService identityService,
            ICurrentUserService currentUserService,
            IDateTime dateTime,
            IOptions<Configuration> options)
            => (_context, _accountService, _exchangeRateService, _identityService, _currentUserService, _dateTime, _configuration)
                = (context, accountService, exchangeRateService, identityService, currentUserService, dateTime, options.Value);

        public Task<decimal> GenerateDefaultReward()
            => Task.FromResult(RandomInRange(_configuration.DefaultRange[0], _configuration.DefaultRange[1]));

        public async Task<(decimal Amount, MiningType Type)> GenerateWeeklyReward()
        {
            var balance = await GetInternalBalance();
            var amount = await GenerateDefaultReward();
            var type = MiningType.Default;

            if (balance > _configuration.BalanceTreshold && Random() > _configuration.Weekly.Probability)
            {
                type = MiningType.Weekly;
                var user = await _identityService.GetUser(_currentUserService.UserId);
                amount = balance * _configuration.Weekly.Coefficients[(int)Math.Floor((_dateTime.UtcNow - user.Registered).TotalDays / 7)] / 100;
            }

            return (amount, type);
        }

        public async Task<(decimal Amount, MiningType Type)> GenerateBonusReward()
        {
            var amount = 0M;
            var type = MiningType.Default;

            var txs = _context.MiningRequests
                .Where(x => _currentUserService.UserId == x.CreatedBy)
                .OrderByDescending(x => x.Created);

            var target = txs
                .Where(x => x.Type == MiningType.Weekly || x.Type == MiningType.Bonus || x.Type == MiningType.Default)
                .Take(_configuration.Bonus.StackTimes)
                .Select((x, i) => new { Index = i, Target = x })
                .ToDictionary(x => x.Index, x => x.Target);

            var bonus = txs.FirstOrDefault(x => x.Type == MiningType.Bonus);

            if (target.Count >= _configuration.Bonus.StackTimes
                && target.All(x => x.Key == 0 || x.Value.Created - target[x.Key - 1].Created <= _configuration.Bonus.Window * 2)
                && (bonus == null || bonus.Created < _dateTime.UtcNow - _configuration.Bonus.Timeout)
                && Random() > _configuration.Bonus.Probability)
            {
                type = MiningType.Bonus;
                amount = RandomInRange(_configuration.Bonus.Range[0], _configuration.Bonus.Range[1]) * await GetInternalBalance();
            }

            return (amount, type);
        }

        public async Task<decimal> GenerateInstantReward()
        {
            var bine = 0M;
            if (Random() < _configuration.Instant.Probability)
            {
                var rate = await _exchangeRateService.GetExchangeRate(new Pair(Currency.BINE, Currency.EURB));
                bine = _configuration.Instant.Fee * rate.Rate;
                var rnd = Random();

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

        public async Task<Promotion> GeneratePromotion(int index)
        {
            // TODO: remove magic numbers.
            var promotion = null as Promotion;
            var probability = _configuration.Promotion.Probability - (index % 5) * 0.01M;

            if (probability < 0.1M)
            {
                probability = 0.1M;
            }

            if (Random() < probability)
            {
                promotion = new Promotion { Id = Guid.NewGuid() };
                var rnd = Random();

                foreach (var currency in _configuration.Promotion.Currencies.OrderBy(x => x.Value))
                {
                    if (rnd <= currency.Value)
                    {
                        promotion.Currency = currency.Key;
                        break;
                    }
                }

                rnd = Random();
                var balance = await _accountService.GetBalance(_currentUserService.UserId, Currency.BINE);
                var last = (await _accountService.GetTransactions(_currentUserService.UserId)).OrderByDescending(x => x.DateTime).First().Amount;

                foreach (var category in _configuration.Promotion.Categories.OrderBy(x => x.Value))
                {
                    if (rnd <= category.Value)
                    {
                        promotion.TokenAmount = category.Key switch
                        {
                            Configuration.PromotionItem.Category.LastRange => last * RandomInRange(0.4M, 0.75M),
                            Configuration.PromotionItem.Category.LastAll => last,
                            Configuration.PromotionItem.Category.AllRange => balance * RandomInRange(0.1M, 05M),
                            _ => throw new NotSupportedException(),
                        };

                        promotion.CurrencyAmount = promotion.TokenAmount * (await _exchangeRateService.GetExchangeRate(new Pair(promotion.Currency, Currency.BINE))).Rate;
                        break;
                    }
                }
            }

            return await Task.FromResult(promotion);
        }

        public Task<decimal> GetInstantMiningFee()
            => Task.FromResult(_configuration.Instant.Fee);

        public Task<TimeSpan> GetMiningRequestWindow()
            => Task.FromResult(_configuration.MiningRequestWindow);

        private async Task<decimal> GetInternalBalance()
        {
            var bine = await _accountService.GetBalance(_currentUserService.UserId, Currency.BINE);
            var eurb = await _accountService.GetBalance(_currentUserService.UserId, Currency.EURB);
            var ex = await _exchangeRateService.GetExchangeRate(new Pair(Currency.BINE, Currency.EURB));
            var balance = eurb + bine * ex.Rate;
            return balance;
        }

        private decimal RandomInRange(decimal start, decimal end)
            => Random() * (end - start) + start;

        private decimal Random() => (decimal)new Random().NextDouble();

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
                public decimal Probability { get; set; }
            }

            public class BonusItem
            {
                public TimeSpan Timeout { get; set; }
                public TimeSpan Window { get; set; }
                public int StackTimes { get; set; }
                public decimal Probability { get; set; }
                public decimal[] Range { get; set; }
            }

            public class InstantItem
            {
                public TimeSpan Timeout { get; set; }
                public Dictionary<string, int> BoostMapping { get; set; }
                public decimal Probability { get; set; }
                public decimal Fee { get; set; }
                public Dictionary<Category, decimal> Categories { get; set; }
                public int OperationLockMiningCount { get; set; }

                public enum Category
                {
                    x2,
                    x2x5,
                    x5x10,
                    x10x100
                }
            }

            public class PromotionItem
            {
                public decimal Probability { get; set; }
                public Dictionary<Currency, decimal> Currencies { get; set; }
                public Dictionary<Category, decimal> Categories { get; set; }

                public enum Category
                {
                    LastRange,
                    LastAll,
                    AllRange
                }
            }
        }
    }
}
