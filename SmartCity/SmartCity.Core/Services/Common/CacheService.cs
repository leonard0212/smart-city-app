using SmartCity.Interfaces.Services.Common;
using Microsoft.Extensions.Caching.Memory;

namespace SmartCity.Core.Services.Common
{
    public class CacheService : ICacheService
    {
        private static readonly object _lock = new object();
        private readonly IMemoryCache _cache;
        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void SetData<TEntity>(string key, TEntity value, int expiresIn) where TEntity : class
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(expiresIn));

            _cache.Set(key, value, cacheEntryOptions);
        }

        public TEntity GetValue<TEntity>(string key) where TEntity : class
        {
            TEntity value = null;
            if (_cache.TryGetValue(key, out value))
                return value;

            return null;
        }






    }
}
