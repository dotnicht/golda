using System;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface ICacheClient
    {
        Task Set<T>(string key, T value, TimeSpan? expiration = null) where T : class;
        Task Set(string key, string value, TimeSpan? expiration = null);
        Task<T> Get<T>(string key) where T : class;
        Task<string> Get(string key);
        Task AddToList<T>(string key, T value) where T : class;
        Task AddToList(string key, string value);
        Task<T> GetLastFromList<T>(string key) where T : class;
        Task<string> GetLastFromList(string key);
        Task<T[]> GetList<T>(string key) where T : class;
        Task<string[]> GetList(string key);
    }
}
