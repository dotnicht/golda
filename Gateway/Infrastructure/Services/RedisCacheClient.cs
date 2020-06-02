using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Infrastructure.Services
{
    public sealed class RedisCacheClient : ICacheClient, IDisposable
    {
        private readonly ILogger _logger;
        private readonly Lazy<ConnectionMultiplexer> _redis;
        private readonly Lazy<RedLockFactory> _redlock;

        public RedisCacheClient(ILogger<RedisCacheClient> logger, IOptions<Redis> options)
        {
            _logger = logger;
            _redis = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options.Value.ConnectionString));

            //var azureEndPoint = new RedLockEndPoint
            //{
            //    EndPoint = new DnsEndPoint("YOUR_CACHE.redis.cache.windows.net", 6380),
            //    Password = "YOUR_ACCESS_KEY",
            //    Ssl = true
            //};

            _redlock = new Lazy<RedLockFactory>(() => RedLockFactory.Create(new[] { (RedLockMultiplexer)_redis.Value }.ToList()));
        }

        public Task Set<T>(string key, T value, TimeSpan? expiration = null) where T : class
            => Set(key, JsonConvert.SerializeObject(value), expiration);

        public async Task Set(string key, string value, TimeSpan? expiration = null)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _logger.LogDebug("Cache key {key} item set {value} with {expiration} expiration.", key, value, expiration ?? Timeout.InfiniteTimeSpan);
            await _redis.Value.GetDatabase().StringSetAsync(key, value, expiration);
        }

        public async Task<T> Get<T>(string key) where T : class
        {
            var item = await Get(key);
            return item == null
                ? null
                : JsonConvert.DeserializeObject<T>(item);
        }

        public async Task<string> Get(string key)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return await _redis.Value.GetDatabase().StringGetAsync(key);
        }

        public async Task<IDisposable> Lock(string name)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var redlock = await _redlock.Value.CreateLockAsync(name, TimeSpan.FromHours(1));
            return redlock.IsAcquired ? redlock : null;
        }

        public void Dispose()
        {
            _redis?.Value.Dispose();
            _redlock?.Value.Dispose();
        }
    }
}
