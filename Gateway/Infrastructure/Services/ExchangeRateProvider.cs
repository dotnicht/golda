using Binance.Net.Interfaces;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Infrastructure.Services
{
    public class ExchangeRateProvider : IExchangeRateProvider, ISingleton<IExchangeRateProvider>
    {
        private readonly IBinanceClient _binanceClient;
        private readonly IBinanceSocketClient _binanceSocketClient;
        private readonly IDateTime _dateTime;
        private readonly ILogger _logger;

        public ExchangeRateProvider(IBinanceClient binanceClient, IBinanceSocketClient binanceSocketClient, IDateTime dateTime, ILogger<ExchangeRateProvider> logger)
            => (_binanceClient, _binanceSocketClient, _dateTime, _logger) = (binanceClient, binanceSocketClient, dateTime, logger);

        public async Task<ExchangeRate> GetExchangeRate(Pair pair)
        {
            if (pair is null)
            {
                throw new ArgumentNullException(nameof(pair));
            }

            var symbol = $"{pair.Base}{pair.Quote}";
            var result = await _binanceClient.GetPriceAsync(symbol);
            return new ExchangeRate { Pair = pair, DateTime = _dateTime.UtcNow, Rate = result.Data.Price };
        }

        public async Task Subscribe(Pair pair, Action<ExchangeRate> handle)
        {
            if (pair is null)
            {
                throw new ArgumentNullException(nameof(pair));
            }

            if (handle is null)
            {
                throw new ArgumentNullException(nameof(handle));
            }

            var symbol = $"{pair.Base}{pair.Quote}";

            _binanceSocketClient.SubscribeToSymbolTickerUpdates(symbol, x =>
            {
                var rate = new ExchangeRate { Pair = pair, Rate = x.CurrentDayClosePrice, DateTime = _dateTime.UtcNow };
                handle(rate);
            });

            _logger.LogInformation($"Subscribed to {symbol} Binance ticker.");

            await Task.CompletedTask;
        }
    }
}
