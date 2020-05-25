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
    public class ExchangeRateProvider : IExchangeRateProvider
    {
        private readonly IBinanceSocketClient _binanceSocketClient;
        private readonly IDateTime _dateTime;
        private readonly ILogger _logger;

        public ExchangeRateProvider(IBinanceSocketClient binanceSocketClient, IDateTime dateTime, ILogger<ExchangeRateProvider> logger)
            => (_binanceSocketClient, _dateTime, _logger) = (binanceSocketClient, dateTime, logger);

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
                var rate = new ExchangeRate { Base = pair.Base, Quote = pair.Quote, Rate = x.CurrentDayClosePrice, DateTime = _dateTime.UtcNow };
                handle(rate);
            });

            _logger.LogInformation("Subscribed to {symbol} Binance ticker.", symbol);

            await Task.CompletedTask;
        }
    }
}
