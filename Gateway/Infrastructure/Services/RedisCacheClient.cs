using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Infrastructure.Services
{
    public sealed class RedisCacheClient : ICacheClient, IDisposable
    {
        private readonly ILogger _logger;
        private readonly Redis _configuration;
        private ConnectionMultiplexer _redis;

        public RedisCacheClient(ILogger<RedisCacheClient> logger, IOptions<Redis> options) 
            => (_logger, _configuration) = (logger, options.Value);

        public Task Set<T>(string key, T value, TimeSpan? expiration = null) where T : class
            => Set(key, JsonConvert.SerializeObject(value), expiration);

        public async Task Set(string key, string value, TimeSpan? expiration = null)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _logger.LogDebug("Cache key {key} item set {value} with {expiration} expiration.", key, value, expiration ?? Timeout.InfiniteTimeSpan);
            await GetDatabase().StringSetAsync(key, value, expiration);
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

            return await GetDatabase().StringGetAsync(key);
        }

        public void Dispose() => _redis?.Dispose();

        private IDatabase GetDatabase()
        {
            if (_redis == null || !_redis.IsConnected || !_redis.IsConnecting)
            {
                try
                {
                    _redis = ConnectionMultiplexer.Connect(_configuration.ConnectionString);
                }
                catch (RedisConnectionException ex)
                {
                    _logger.LogError(ex, "An error occurred while connecting to Redis instance.");
                    throw;
                }
            }

            if (_redis == null || !_redis.IsConnected)
            {
                throw new InvalidOperationException("Not connected to Redis server.");
            }

            return _redis.GetDatabase();
        }
    }
}
