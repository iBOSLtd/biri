namespace PeopleDesk.Services.Cache
{
    public interface IDistributedCacheService
    {
        Task<IEnumerable<T>> AddDataToCache<T>(string key, IEnumerable<T> data, int setAbsoluteExpiration, int setSlidingExpiration, string casheExperiationType) where T : class;
        Task<IEnumerable<T>> DataFromCache<T>(string key) where T : class;
        Task<bool> HasCache(string key);
        Task<bool> RemoveCache(string key);
    }
}
