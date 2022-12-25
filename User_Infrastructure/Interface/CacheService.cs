using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace User_Infrastructure.Interface
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<bool> IsConnectedAsync(string key)
        {
            try
            {
                byte[] value = await _cache.GetAsync(key);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<T>?> GetCachedObject<T>(string cacheKeyPrefix)
        {
            try
            {
                // Construct the key for the cache
                string cacheKey = $"{cacheKeyPrefix}";
                if (await IsConnectedAsync(cacheKey))
                {
                    // Get the cached item
                    string cachedObjectJson = await _cache.GetStringAsync(cacheKey);

                    // If there was a cached item then deserialise this
                    if (!string.IsNullOrEmpty(cachedObjectJson))
                    {
                        List<T> cachedObject = JsonConvert.DeserializeObject<List<T>>(cachedObjectJson);
                        return cachedObject;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error in CacheService.CS :- " + ex.Message, ex.InnerException);
            }

            return default;
        }

        public async Task<bool> SetCachedObject(string cacheKeyPrefix, dynamic objectToCache, double AbsoluteExpirationInHours, double SlidingExpirationInMinutes)
        {
            if (await IsConnectedAsync(cacheKeyPrefix))
            {
                string cachedObjectJson = await _cache.GetStringAsync(cacheKeyPrefix);
                if (!string.IsNullOrEmpty(cachedObjectJson))
                {
                    _ = await RemoveCache(cacheKeyPrefix);
                }
                try
                {
                    string catchdata = JsonConvert.SerializeObject(objectToCache);
                    byte[] redisCustomerList = Encoding.UTF8.GetBytes(catchdata);

                    string cacheKey = $"{cacheKeyPrefix}";

                    DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(DateTime.Now.AddMinutes(AbsoluteExpirationInHours))
                        .SetSlidingExpiration(TimeSpan.FromMinutes(SlidingExpirationInMinutes));
                    await _cache.SetAsync(cacheKey, redisCustomerList, options);

                    return true;
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Error in CacheService.CS :- " + ex.Message, ex.InnerException);
                }
            }
            return false;
        }

        public async Task<bool> CheckExist(string cacheKeyPrefix)
        {
            string? redisdata = await _cache.GetStringAsync($"{cacheKeyPrefix}");
            return redisdata != null;
        }

        public async Task<bool> RemoveCache(string cacheKeyPrefix)
        {
            if (await IsConnectedAsync(cacheKeyPrefix))
            {
                await _cache.RemoveAsync($"{cacheKeyPrefix}");
                return true;
            }
            return false;
        }
    }
}