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

namespace Binebase.Exchange.Gateway.Application.Services
{
    public sealed class ExchangeRateService : IExchangeRateService, IConfigurationProvider<ExchangeRateService.Configuration>, ISingleton<IExchangeRateService>, IDisposable
    {
        private readonly Configuration _configuration;
        private readonly ILogger _logger;
        private readonly IDateTime _dateTime;
        private readonly ICacheClient _cacheClient;
        private readonly IExchangeRateProvider _exchangeRateProvider;
        private Timer _timer;

        private readonly Pair[] _supportedPairs;

        public ExchangeRateService(
            IOptions<Configuration> options,
            ILogger<ExchangeRateService> logger,
            IDateTime dateTime,
            ICacheClient cacheClient,
            IExchangeRateProvider exchangeRateProvider)
            => (_configuration, _logger, _dateTime, _cacheClient, _exchangeRateProvider, _supportedPairs)
                = (options.Value, logger, dateTime, cacheClient, exchangeRateProvider, options.Value.SupportedPairs.Select(x => Pair.Parse(x)).ToArray());

        public async Task<ExchangeRate> GetExchangeRate(Pair pair, bool forceSupported = true)
        {
            if (pair is null)
            {
                throw new ArgumentNullException(nameof(pair));
            }

            if (forceSupported && !_supportedPairs.Contains(pair))
            {
                throw new NotSupportedException($"Supported currency pairs: {string.Join(' ', _supportedPairs.AsEnumerable())}.");
            }

            var key = pair.ToString();
            var value = await _cacheClient.GetLastFromList<ExchangeRate>(key);

            if (value == null)
            {
                if (_configuration.AllowFallbackClientRequest)
                {
                    value = await _exchangeRateProvider.GetExchangeRate(pair);
                    if (_configuration.CacheFallbackResult)
                    {
                        await _cacheClient.AddToList(key, value);
                    }
                }
            }

            return value;
        }

        public async Task<ExchangeRate[]> GetExchangeRateHistory(Pair pair)
        {
            if (pair is null)
            {
                throw new ArgumentNullException(nameof(pair));
            }

            if (!_supportedPairs.Contains(pair))
            {
                throw new NotSupportedException($"Supported currency pairs: {string.Join(' ', _supportedPairs.AsEnumerable())}.");
            }

            var key = pair.ToString();
            return await _cacheClient.GetList<ExchangeRate>(key);
        }

        public Task<Dictionary<Pair, ExchangeRate>> GetExchangeRates()
            => Task.FromResult(_supportedPairs.ToDictionary(x => x, x => GetExchangeRate(x).Result));

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

            try
            {
                _cacheClient.AddToList(rate.Pair.ToString(), rate).Wait();
            }
            catch (AggregateException ex)
            {
                _logger.LogError(ex, "Error adding exchange rate to cache.");
            }
        }

        public void Dispose() => _timer?.Dispose();

        public class Configuration
        {
            public bool AllowFallbackClientRequest { get; set; }
            public bool CacheFallbackResult { get; set; }
            public string[] SupportedPairs { get; set; }
            public decimal BineBaseValue { get; set; }
            public decimal[] BineRange { get; set; }
            public TimeSpan BineRefreshRate { get; set; }
            public decimal ExchangeFee { get; set; }
        }
    }
}
