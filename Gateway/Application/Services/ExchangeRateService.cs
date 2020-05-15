using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Application.Configuration;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Services
{
    public sealed class ExchangeRateService : IExchangeRateService, IDisposable
    {
        private readonly ExchangeRates _configuration;
        private readonly IDateTime _dateTime;
        private readonly IExchangeRateProvider _exchangeRateProvider;
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        private readonly Pair[] _supportedPairs;
        private readonly Pair[] _backwardPairs;
        private readonly Pair[] _exchangeExcludePairs;

        public ExchangeRateService(
            IOptions<ExchangeRates> options,
            IDateTime dateTime,
            IExchangeRateProvider exchangeRateProvider,
            IServiceProvider serviceProvider)
            => (_configuration, _dateTime, _exchangeRateProvider, _serviceProvider, _supportedPairs, _backwardPairs, _exchangeExcludePairs)
                = (options.Value,
                   dateTime,
                   exchangeRateProvider,
                   serviceProvider,
                   options.Value.SupportedPairs.Select(x => Pair.Parse(x)).ToArray(),
                   options.Value.SupportedPairs.Select(x => Pair.Parse(x)).Select(x => new Pair(x.Quote, x.Base)).ToArray(),
                   options.Value.ExchangeExcludePairs.Select(x => Pair.Parse(x)).ToArray());

        public async Task<ExchangeRate> GetExchangeRate(Pair pair, bool forceSupported = true, bool forceExchange = false)
        {
            if (pair is null)
            {
                throw new ArgumentNullException(nameof(pair));
            }

            if (forceSupported
                && !_supportedPairs.Contains(pair)
                && (_configuration.SupportBackward && !_backwardPairs.Contains(pair))
                || forceExchange
                && _exchangeExcludePairs.Contains(pair))
            {
                throw new NotSupportedException(ErrorCode.ExchangeRateNotSupported);
            }

            using var scope = _serviceProvider.CreateScope();
            using var ctx = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            var rates = ctx.ExchangeRates.OrderByDescending(x => x.DateTime);

            var rate = await rates.FirstOrDefaultAsync(x => x.Base == pair.Base && x.Quote == pair.Quote);

            if (rate == null)
            {
                var first = await rates.FirstOrDefaultAsync(x => x.Base == pair.Base && x.Quote == Currency.EURB)
                    ?? throw new NotSupportedException(ErrorCode.ExchangeRateNotSupported);
                var second = await rates.FirstOrDefaultAsync(x => x.Base == Currency.EURB && x.Quote == pair.Quote)
                    ?? throw new NotSupportedException(ErrorCode.ExchangeRateNotSupported);

                rate = new ExchangeRate
                {
                    Base = pair.Base,
                    Quote = pair.Quote,
                    DateTime = _dateTime.UtcNow,
                    Rate = first.Rate * second.Rate
                };
            }

            return rate;
        }

        public async Task<ExchangeRate[]> GetExchangeRateHistory(Pair pair)
        {
            if (pair is null)
            {
                throw new ArgumentNullException(nameof(pair));
            }

            if (!_supportedPairs.Contains(pair))
            {
                throw new NotSupportedException(ErrorCode.ExchangeRateNotSupported);
            }

            using var scope = _serviceProvider.CreateScope();
            using var ctx = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            var key = pair.ToString();
            return await ctx.ExchangeRates.Where(x => x.Base == pair.Base && x.Quote == pair.Quote).ToArrayAsync();
        }

        public async Task<ExchangeRate[]> GetExchangeRates()
        {
            var result = _supportedPairs.Select(x => GetExchangeRate(x).Result);
            if (_configuration.SupportBackward)
            {
                result = result.Union(_backwardPairs.Select(x => GetExchangeRate(x).Result));
            }

            return await Task.FromResult(result.ToArray());
        }

        public async Task Subscribe()
        {
            await _exchangeRateProvider.Subscribe(new Pair(Currency.BTC, Currency.EUR), SaveExchangeRate);
            await _exchangeRateProvider.Subscribe(new Pair(Currency.ETH, Currency.EUR), SaveExchangeRate);
            _timer = new Timer(Refresh, null, TimeSpan.Zero, _configuration.BineRefreshRate);
        }

        private async void SaveExchangeRate(ExchangeRate rate)
        {
            if (rate is null)
            {
                throw new ArgumentNullException(nameof(rate));
            }

            using var scope = _serviceProvider.CreateScope();
            using var ctx = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            rate.Quote = Currency.EURB;
            ctx.ExchangeRates.Add(rate);
            ctx.ExchangeRates.Add(new ExchangeRate { Base = Currency.EURB, Quote = rate.Base, DateTime = rate.DateTime, Rate = (1 - _configuration.ExchangeFee) / rate.Rate });
            await ctx.SaveChangesAsync();
        }

        private async void Refresh(object state)
        {
            // TODO: move BINE exchange rate numbers to config.

            var rate = new ExchangeRate
            {
                Base = Currency.BINE,
                Quote = Currency.EURB,
                DateTime = _dateTime.UtcNow,
                Rate = _configuration.BineBaseValue
            };

            if (new Random().NextDouble() > 0.5)
            {
                var delta = rate.Rate * (decimal)new Random().NextDouble() * 0.2M;
                rate.Rate -= delta;
                if (rate.Rate < _configuration.BineRange[0])
                {
                    rate.Rate = _configuration.BineRange[0] + delta;
                }
            }
            else
            {
                var delta = rate.Rate * (decimal)new Random().NextDouble() * 0.3M;
                rate.Rate += delta;
                if (rate.Rate > _configuration.BineRange[1])
                {
                    rate.Rate = _configuration.BineRange[1] - delta;
                }
            }

            var backward = new ExchangeRate
            {
                Base = Currency.EURB,
                Quote = Currency.BINE,
                DateTime = _dateTime.UtcNow,
                Rate = 1 / rate.Rate
            };

            using var scope = _serviceProvider.CreateScope();
            using var ctx = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            ctx.ExchangeRates.Add(rate);
            ctx.ExchangeRates.Add(backward);
            await ctx.SaveChangesAsync();
        }

        public void Dispose() => _timer?.Dispose();
    }
}
