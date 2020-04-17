using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binebase.Exchange.Gateway.Application.Configuration;

namespace Binebase.Exchange.Gateway.Application.Services
{
    public sealed class ExchangeRateService : IExchangeRateService, IDisposable
    {
        private readonly ExchangeRates _configuration;
        private readonly ILogger _logger;
        private readonly IDateTime _dateTime;
        private readonly ICacheClient _cacheClient;
        private readonly IExchangeRateProvider _exchangeRateProvider;
        private Timer _timer;

        private readonly Pair[] _supportedPairs;
        private readonly Pair[] _backwardPairs;
        private readonly Pair[] _exchangeExcludePairs;

        public ExchangeRateService(
            IOptions<ExchangeRates> options,
            ILogger<ExchangeRateService> logger,
            IDateTime dateTime,
            ICacheClient cacheClient,
            IExchangeRateProvider exchangeRateProvider)
            => (_configuration, _logger, _dateTime, _cacheClient, _exchangeRateProvider, _supportedPairs, _backwardPairs, _exchangeExcludePairs)
                = (options.Value,
                   logger, dateTime,
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

            var rate = await _cacheClient.GetLastFromList<ExchangeRate>(pair.ToString());
            if (rate == null)
            {
                var first = await _cacheClient.GetLastFromList<ExchangeRate>(new Pair(pair.Base, Currency.EURB).ToString());
                var second = await _cacheClient.GetLastFromList<ExchangeRate>(new Pair(Currency.EURB, pair.Quote).ToString());

                rate = new ExchangeRate
                {
                    Pair = pair,
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

            var key = pair.ToString();
            return await _cacheClient.GetList<ExchangeRate>(key);
        }

        public Task<ExchangeRate[]> GetExchangeRates()
            => Task.FromResult(_supportedPairs.Select(x => GetExchangeRate(x).Result).ToArray());

        public async Task Subscribe()
        {
            await _exchangeRateProvider.Subscribe(new Pair(Currency.BTC, Currency.EUR), SaveExchangeRate);
            await _exchangeRateProvider.Subscribe(new Pair(Currency.ETH, Currency.EUR), SaveExchangeRate);

            _timer = new Timer(Refresh, null, TimeSpan.Zero, _configuration.BineRefreshRate);
        }

        private void SaveExchangeRate(ExchangeRate rate)
        {
            _cacheClient.AddToList(new Pair(rate.Pair.Base, Currency.EURB).ToString(), rate);
            var symbol = new Pair(Currency.EURB, rate.Pair.Base);
            _cacheClient.AddToList(symbol.ToString(), new ExchangeRate { Pair = symbol, DateTime = rate.DateTime, Rate = (1 + _configuration.ExchangeFee) / rate.Rate });
        }

        private void Refresh(object state)
        {
            // TODO: move BINE exchange rate numbers to config.

            var rate = new ExchangeRate
            {
                Pair = new Pair(Currency.BINE, Currency.EURB),
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
                Pair = new Pair(Currency.EURB, Currency.BINE),
                DateTime = _dateTime.UtcNow,
                Rate = (1 - _configuration.ExchangeFee) / rate.Rate
            };

            try
            {
                Task.WaitAll(_cacheClient.AddToList(rate.Pair.ToString(), rate), _cacheClient.AddToList(backward.Pair.ToString(), backward));
            }
            catch (AggregateException ex)
            {
                _logger.LogError(ex, "Error adding exchange rate to cache.");
            }
        }

        public void Dispose() => _timer?.Dispose();
    }
}
