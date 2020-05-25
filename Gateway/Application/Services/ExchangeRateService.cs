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
        private readonly ICacheClient _cacheClient;
        private readonly IExchangeRateProvider _exchangeRateProvider;
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        private readonly Pair[] _supportedPairs;
        private readonly Pair[] _backwardPairs;
        private readonly Pair[] _exchangeExcludePairs;

        public ExchangeRateService(
            IOptions<ExchangeRates> options,
            IDateTime dateTime,
            ICacheClient cacheClient,
            IExchangeRateProvider exchangeRateProvider)
            => (_configuration, _dateTime, _cacheClient, _exchangeRateProvider, _supportedPairs, _backwardPairs, _exchangeExcludePairs)
                = (options.Value,
                   dateTime,
                   cacheClient,
                   exchangeRateProvider,
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

            return await _cacheClient.Get<ExchangeRate>(pair.ToString());
        }

        public async Task<ExchangeRate[]> GetExchangeRates()
        {
            var result = _supportedPairs.Select(x => GetExchangeRate(x));

            if (_configuration.SupportBackward)
            {
                result = result.Union(_backwardPairs.Select(x => GetExchangeRate(x)));
            }

            await Task.WhenAll(result);
            return result.Select(x => x.Result).ToArray();
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
            return await ctx.ExchangeRates
                .Where(x => x.Base == pair.Base && x.Quote == pair.Quote)
                .OrderByDescending(x => x.DateTime)
                .ToArrayAsync();
        }

        public async Task Subscribe()
        {
            await _exchangeRateProvider.Subscribe(new Pair(Currency.BTC, Currency.EUR), SaveExchangeRate);
            await _exchangeRateProvider.Subscribe(new Pair(Currency.ETH, Currency.EUR), SaveExchangeRate);
            _timer = new Timer(Refresh, null, TimeSpan.Zero, _configuration.BineRefreshRate);
        }

        private void SaveExchangeRate(ExchangeRate rate)
        {
            var symbol = new Pair(rate.Quote, Currency.EURB);
            _cacheClient.Set(symbol.ToString(), rate);
            symbol = new Pair(Currency.EURB, rate.Base);
            _cacheClient.Set(symbol.ToString(), new ExchangeRate { Base = symbol.Base, Quote = symbol.Quote, DateTime = rate.DateTime, Rate = (1 - _configuration.ExchangeFee) / rate.Rate });
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

            await Task.WhenAll(new[] { rate, backward }.Select(x => _cacheClient.Set(new Pair(x.Base, x.Quote).ToString(), x)));

            using var scope = _serviceProvider.CreateScope();
            using var ctx = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            ctx.ExchangeRates.Add(rate);
            ctx.ExchangeRates.Add(backward);
            await ctx.SaveChangesAsync();
        }

        public void Dispose() => _timer?.Dispose();
    }
}
