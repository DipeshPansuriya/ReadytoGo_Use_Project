namespace User_Infrastructure.Interface
{
    public interface ICacheService
    {
        Task<bool> IsConnectedAsync(string key);

        Task<List<T>> GetCachedObject<T>(string cacheKeyPrefix);

        Task<bool> SetCachedObject(string cacheKeyPrefix, dynamic objectToCache, double AbsoluteExpirationInHours, double SlidingExpirationInMinutes);

        Task<bool> CheckExist(string cacheKeyPrefix);

        Task<bool> RemoveCache(string cacheKeyPrefix);
    }
}