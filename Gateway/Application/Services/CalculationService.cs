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
            => Task.FromResult((decimal)new Random().NextDouble() * (_configuration.DefaultRange[1] - _configuration.DefaultRange[0]) + _configuration.DefaultRange[0]);

        public async Task<(decimal Amount, TransactionType Type)> GenerateSimpleMiningReward()
        {
            var bine = await _accountService.GetBalance(_currentUserService.UserId, Currency.BINE);
            var eurb = await _accountService.GetBalance(_currentUserService.UserId, Currency.EURB);
            var ex = await _exchangeRateService.GetExchangeRate(new Pair(Currency.BINE, Currency.EURB));
            var balance = eurb * (1 / ex.Rate) + bine;

            var amount = await GenerateDefaultReward();
            var type = TransactionType.Default;

            if (balance > _configuration.BalanceTreshold)
            {
                var txs = (await _accountService.GetTransactions(_currentUserService.UserId, Currency.BINE)).OrderByDescending(x => x.DateTime);
                var bonus = txs.FirstOrDefault(x => x.Type == TransactionType.Bonus);
                if (bonus?.DateTime < _dateTime.UtcNow - _configuration.BonusCooldown && new Random().NextDouble() > _configuration.Probability.Bonus)
                {
                    var target = txs
                        .Where(x => x.Source == TransactionSource.Mining && x.Type != TransactionType.Bonus)
                        .Take(_configuration.BonusStackTimes)
                        .Select((x, i) => new { Index = i, Target = x })
                        .ToDictionary(x => x.Index, x => x.Target);

                    foreach (var tx in target)
                    {

                    }

                    // TODO: bonus.
                    type = TransactionType.Bonus;
                }

                if (new Random().NextDouble() > _configuration.Probability.Default)
                {
                    var user = await _identityService.GetUser(_currentUserService.UserId);
                    amount = balance * _configuration.WeeklyCoefficients[(int)Math.Floor((_dateTime.UtcNow - user.Registered).TotalDays / 7)];
                }
            }

            return (amount, type);
        }

        public async Task<decimal> GenerateInstantMiningReward()
        {
            var bine = 0M;
            if (new Random().NextDouble() < _configuration.Probability.Instant)
            {
                var rate = await _exchangeRateService.GetExchangeRate(new Pair(Currency.BINE, Currency.EURB));
                bine = _configuration.InstantMiningFee / rate.Rate;

                foreach (var range in _configuration.Probability.InstantMiningRanges.OrderBy(x => x.Key))
                {
                    //if (new Random().NextDouble() <= range.Key)
                    {
                        bine *= (decimal)new Random().NextDouble() * (range.Value[1] - range.Value[0]) + range.Value[0];
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

        public async Task<TimeSpan> GetBonusTimeout()
        {
            var txs = await _accountService.GetTransactions(_currentUserService.UserId, Currency.BINE);
            var last = txs.OrderByDescending(x => x.DateTime).FirstOrDefault(x => x.Type == TransactionType.Bonus && x.Source == TransactionSource.Mining);
            return last == null || last.DateTime <= _dateTime.UtcNow - _configuration.BonusTimeout ? default : _dateTime.UtcNow - last.DateTime;
        }

        public async Task<TimeSpan> GetInstantTimeout()
        {
            var txs = await _accountService.GetTransactions(_currentUserService.UserId, Currency.BINE);
            var last = txs.OrderByDescending(x => x.DateTime).FirstOrDefault(x => x.Type == TransactionType.Instant && x.Source == TransactionSource.Mining);
            return last == null || last.DateTime <= _dateTime.UtcNow - _configuration.InstantTimeout ? default : _dateTime.UtcNow - last.DateTime;
        }

        public async Task<int> GetCurrentMiningCount()
        {
            var txs = await _accountService.GetTransactions(_currentUserService.UserId, Currency.BINE);
            return txs.Count(x => x.Source == TransactionSource.Mining && x.Type == TransactionType.Instant);
        }

        public Task<Dictionary<int, int>> GetInstantBoostMapping()
            => Task.FromResult(_configuration.InstantBoostMapping);

        public Task<decimal> GetInstantMiningFee() 
            => Task.FromResult(_configuration.InstantMiningFee);

        public Task<TimeSpan> GetMiningRequestWindow() 
            => Task.FromResult(_configuration.MiningRequestWindow);

        public class Configuration
        {
            public TimeSpan BonusTimeout { get; set; }
            public TimeSpan InstantTimeout { get; set; }
            public TimeSpan MiningRequestWindow { get; set; }
            public decimal[] DefaultRange { get; set; }
            public decimal[] WeeklyCoefficients { get; set; }
            public decimal BalanceTreshold { get; set; }
            public decimal[] BonusRange { get; set; }
            public TimeSpan BonusTimeThreshold { get; set; }
            public TimeSpan BonusCooldown { get; set; }
            public int BonusStackTimes { get; set; }
            public Dictionary<int, int> InstantBoostMapping { get; set; }
            public decimal InstantMiningFee { get; set; }
            public ProbabilityItem Probability { get; set; }
            public PromotionItem Promotion { get; set; }

            public class ProbabilityItem
            {
                public double Default { get; set; }
                public double Bonus { get; set; }
                public double Instant { get; set; }
                public Dictionary<double[], int[]> InstantMiningRanges { get; set; }
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
