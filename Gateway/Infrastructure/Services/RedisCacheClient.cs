using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Common.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Binebase.Exchange.Gateway.Infrastructure.Services
{
    public sealed class RedisCacheClient : ICacheClient, IDisposable, IConfigurationProvider<RedisCacheClient.Configuration>, ISingleton<ICacheClient>
    {
        private readonly ILogger _logger;
        private readonly Configuration _configuration;
        private ConnectionMultiplexer _redis;

        public RedisCacheClient(ILogger<RedisCacheClient> logger, IOptions<Configuration> options) 
            => (_logger, _configuration) = (logger, options.Value);

        public Task Set<T>(string key, T value, TimeSpan? expiration = null) where T : class
            => Set(key, JsonConvert.SerializeObject(value), expiration);

        public async Task Set(string key, string value, TimeSpan? expiration = null)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _logger.LogDebug($"Cache key {key} item set {value} with {expiration ?? Timeout.InfiniteTimeSpan} expiration.");
            await GetDatabase().StringSetAsync(key, value);
        }

        public async Task<T> Get<T>(string key) where T : class
            => JsonConvert.DeserializeObject<T>(await Get(key));

        public async Task<string> Get(string key)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return await GetDatabase().StringGetAsync(key);
        }

        public Task AddToList<T>(string key, T value) where T : class
            => AddToList(key, JsonConvert.SerializeObject(value));

        public async Task AddToList(string key, string value)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _logger.LogDebug($"Cache key {key} item added {value}.");
            await GetDatabase().ListRightPushAsync(key, value);
        }

        public async Task<T> GetLastFromList<T>(string key) where T : class
        {
            var value = await GetLastFromList(key);
            return value == null 
                ? null as T 
                : JsonConvert.DeserializeObject<T>(value);
        }

        public async Task<string> GetLastFromList(string key)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var db = GetDatabase();
            var length = db.ListLength(key);
            var range = await db.ListRangeAsync(key, length - 1, length);
            return range.SingleOrDefault();
        }

        public async Task<T[]> GetList<T>(string key) where T : class
            => (await GetList(key)).Select(x => JsonConvert.DeserializeObject<T>(x)).ToArray();

        public async Task<string[]> GetList(string key)
        {
            var db = GetDatabase();
            var range = await db.ListRangeAsync(key, 0, db.ListLength(key));
            return range.Select(x => x.ToString()).ToArray();
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
                    _logger.LogError(ex, $"An error occurred while connecting to Redis instance.");
                    throw;
                }
            }

            if (_redis == null || !_redis.IsConnected)
            {
                throw new InvalidOperationException("Not connected to Redis server.");
            }

            return _redis.GetDatabase();
        }

        public class Configuration
        {
            public string ConnectionString { get; set; }
        }
    }
}
