using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Application.Configuration;
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
    public class CalculationService : ICalculationService
    {
        private readonly IApplicationDbContext _context;
        private readonly IAccountService _accountService;
        private readonly IExchangeRateService _exchangeRateService;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;
        private readonly MiningCalculation _configuration;

        public CalculationService(
            IApplicationDbContext context,
            IAccountService accountService,
            IExchangeRateService exchangeRateService,
            IIdentityService identityService,
            ICurrentUserService currentUserService,
            IDateTime dateTime,
            IOptions<MiningCalculation> options)
            => (_context, _accountService, _exchangeRateService, _identityService, _currentUserService, _dateTime, _configuration)
                = (context, accountService, exchangeRateService, identityService, currentUserService, dateTime, options.Value);

        public Task<decimal> GenerateDefaultReward()
            => Task.FromResult(Random(_configuration.DefaultRange[0], _configuration.DefaultRange[1]));

        public async Task<(decimal Amount, MiningType Type)> GenerateWeeklyReward()
        {
            var balance = await GetInternalBalance() * (await _exchangeRateService.GetExchangeRate(new Pair(Currency.EURB, Currency.BINE))).Rate;
            var amount = await GenerateDefaultReward();
            var type = MiningType.Default;

            if (balance > _configuration.BalanceTreshold && Random() > _configuration.Weekly.Probability)
            {
                type = MiningType.Weekly;

                var user = await _identityService.GetUser(_currentUserService.UserId);
                var index = (int)Math.Floor((_dateTime.UtcNow - user.Registered).TotalDays / 7);
                var coef = index < _configuration.Weekly.Coefficients.Length ? _configuration.Weekly.Coefficients[index] : _configuration.Weekly.Coefficients.Last();
                var value = balance * coef / 100;
                var existing = _context.MiningRequests
                    .Where(x => _currentUserService.UserId == x.CreatedBy && x.Created >= _dateTime.UtcNow - TimeSpan.FromDays(7) && x.Type == MiningType.Weekly)
                    .Sum(x => x.Amount);

                if (existing <= value)
                {
                    amount = Random(0M, value - existing);
                }
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
                .ToArray()
                .Select((x, i) => new { Index = i, Target = x })
                .ToDictionary(x => x.Index, x => x.Target);

            var bonus = txs.FirstOrDefault(x => x.Type == MiningType.Bonus);

            if (target.Count >= _configuration.Bonus.StackTimes
                && target.All(x => x.Key == 0 || x.Value.Created - target[x.Key - 1].Created <= _configuration.Bonus.Window * 2)
                && (bonus == null || bonus.Created < _dateTime.UtcNow - _configuration.Bonus.Timeout)
                && Random() > _configuration.Bonus.Probability)
            {
                type = MiningType.Bonus;
                amount = Random(_configuration.Bonus.Range[0], _configuration.Bonus.Range[1]) * await GetInternalBalance();
            }

            return (amount, type);
        }

        public async Task<decimal> GenerateInstantReward()
        {
            var bine = 0M;
            if (Random() < _configuration.Instant.Probability)
            {
                var rate = await _exchangeRateService.GetExchangeRate(new Pair(Currency.BINE, Currency.EURB));
                bine = _configuration.Instant.Fee / rate.Rate;
                var rnd = Random();

                foreach (var range in _configuration.Instant.Categories.OrderBy(x => x.Value))
                {
                    if (rnd <= range.Value)
                    {
                        bine *= range.Key switch
                        {
                            MiningCalculation.InstantItem.Category.x2 => 2,
                            MiningCalculation.InstantItem.Category.x2x5 => Random(2, 5),
                            MiningCalculation.InstantItem.Category.x5x10 => Random(5, 10),
                            MiningCalculation.InstantItem.Category.x10x100 => Random(10, 100),
                            _ => throw new InvalidOperationException(),
                        };

                        break;
                    }
                }
            }

            return bine;
        }

        public async Task<Promotion> GeneratePromotion(int index, decimal last)
        {
            // TODO: remove magic numbers.
            var probability = _configuration.Promotion.Probability - (index % 5) * 0.01M;

            if (probability < 0.1M)
            {
                probability = 0.1M;
            }

            var promotion = null as Promotion;

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

                foreach (var category in _configuration.Promotion.Categories.OrderBy(x => x.Value))
                {
                    if (rnd <= category.Value)
                    {
                        promotion.TokenAmount = category.Key switch
                        {
                            MiningCalculation.PromotionItem.Category.LastRange => last * Random(0.4M, 0.75M),
                            MiningCalculation.PromotionItem.Category.LastAll => last,
                            MiningCalculation.PromotionItem.Category.AllRange => balance * Random(0.1M, 05M),
                            _ => throw new InvalidOperationException(),
                        };

                        promotion.CurrencyAmount = promotion.TokenAmount * (await _exchangeRateService.GetExchangeRate(new Pair(Currency.BINE, promotion.Currency), false)).Rate;
                        break;
                    }
                }
            }

            return promotion;
        }

        private async Task<decimal> GetInternalBalance()
        {
            var bine = await _accountService.GetBalance(_currentUserService.UserId, Currency.BINE);
            var eurb = await _accountService.GetBalance(_currentUserService.UserId, Currency.EURB);
            var ex = await _exchangeRateService.GetExchangeRate(new Pair(Currency.BINE, Currency.EURB));
            var balance = eurb + bine * ex.Rate;
            return balance;
        }

        private decimal Random(decimal start, decimal end)
            => Random() * (end - start) + start;

        private decimal Random() => (decimal)new Random().NextDouble();
    }
}
