using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Infrastructure.Services
{
    public sealed class RedisCacheClient : ICacheClient, IDisposable, IConfigurationProvider<RedisCacheClient.Configuration>, ISingleton<ICacheClient>
    {
        private readonly ILogger _logger;
        private readonly Configuration _configuration;
        private ConnectionMultiplexer _redis;

        public RedisCacheClient(ILogger<RedisCacheClient> logger, IOptions<Configuration> options) 
            => (_logger, _configuration) = (logger, options.Value);

        public Task Set<T>(string key, T value) where T : class
            => Set(key, JsonConvert.SerializeObject(value));

        public async Task Set(string key, string value)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            await GetDatabase().StringSetAsync(key, value);
        }

        public async Task Get<T>(string key) where T : class
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

        public void Connect()
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
        }

        public void Dispose() => _redis?.Dispose();

        private IDatabase GetDatabase()
        {
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
