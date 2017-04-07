using System;
using System.Runtime.Caching;

namespace MZBlog.Core.Cache
{
    public class RuntimeCache : ICache
    {
        private readonly MemoryCache _cache;
        private readonly CacheItemPolicy _defaultCacheItemPolicy;

        public RuntimeCache()
        {
            _cache = MemoryCache.Default;
            _defaultCacheItemPolicy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromSeconds(60 * 2) };
        }

        public void Add<T>(string key, T obj)
        {
            var cacheItem = new CacheItem(key, obj);
            _cache.Set(cacheItem, _defaultCacheItemPolicy);
        }

        public void Add<T>(string key, T obj, int seconds)
        {
            _cache.Set(key, obj, DateTimeOffset.Now.AddSeconds(seconds));
        }

        public void Add<T>(string key, T obj, TimeSpan slidingExpiration)
        {
            var cacheItem = new CacheItem(key, obj);
            var cacheItemPolicy = new CacheItemPolicy { SlidingExpiration = slidingExpiration };
            _cache.Set(cacheItem, cacheItemPolicy);
        }

        public bool Exists(string key)
        {
            return _cache.Get(key) != null;
        }

        public T Get<T>(string key)
        {
            return (T)_cache.Get(key);
        }

        public void Max<T>(string key, T obj)
        {
            var cacheItem = new CacheItem(key, obj);
            var cacheItemPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTime.MaxValue.AddYears(-1), Priority = CacheItemPriority.NotRemovable };
            _cache.Set(cacheItem, cacheItemPolicy);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public bool Test()
        {
            const string KEY = "_##**Test**##_";
            const string OBJ = "Test";
            Add(KEY, OBJ);
            var result = Get<string>(KEY);
            return result == OBJ;
        }
    }
}