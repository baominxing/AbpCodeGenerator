using System;
using System.Runtime.Caching;

namespace ABPCodeGenerator.Utilities
{
    public static class CacheHelper
    {
        public static void Set(string key, object obj)
        {
            var cache = MemoryCache.Default;

            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration
            };

            cache.Set(key, obj, policy);
        }

        public static T Get<T>(string key) where T : class
        {
            var cache = MemoryCache.Default;

            try
            {
                return (T)cache[key];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void Remove(string key)
        {
            MemoryCache.Default.Remove(key);
        }

    }
}
