using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using PeopleDesk.Models;
using System.Text;

namespace PeopleDesk.Services.Cache
{
    public class CacheExpiration
    {
        public int SetAbsoluteExpiration { get; set; } = 0;
        public int SetSlidingExpiration { get; set; } = 0;
    }
    public interface ICacheService
    {
        IEnumerable<T> AddDataToCache<T>(string query, IEnumerable<T> dbData) where T : class;
        IEnumerable<T> AddDataToCache<T>(string query, IEnumerable<T> dbData, int SetAbsoluteExpiration, int SetSlidingExpiration, string getCacheExpTimefor) where T : class;
        IEnumerable<T> AddDataToCache<T>(string query, IEnumerable<T> dbData, int SetAbsoluteExpiration, int SetSlidingExpiration) where T : class;
        IEnumerable<T> DataFromCache<T>(string query) where T : class;
        bool hasCache(string query);
        bool AddActiveCacheKey(string CacheKeybase, string key, string UserId, bool IsRemoveReq);
        bool RemoveCache(string query);
    }
    public class CacheService : ICacheService
    {
        private static object locked = new object();
        private readonly IDistributedCache _distributedCache;
        public CacheExpiration _cacheExpiration;
        private IConfiguration _configuration;
        public CacheService(IDistributedCache distributedCache, CacheExpiration cacheExpiration, IConfiguration configuration)
        {
            _distributedCache = distributedCache;
            _cacheExpiration = cacheExpiration;
            _configuration = configuration;
            _cacheExpiration.SetAbsoluteExpiration = int.Parse(_configuration.GetSection("CommonCacheExpiration")["SetAbsoluteExpiration"]);
            _cacheExpiration.SetSlidingExpiration = int.Parse(_configuration.GetSection("CommonCacheExpiration")["SetSlidingExpiration"]);
        }
        public bool hasCache(string query)
        {
            var CacheData = _distributedCache.Get(query);
            if (CacheData != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerable<T> AddDataToCache<T>(string query, IEnumerable<T> dbData, int SetAbsoluteExpiration, int SetSlidingExpiration) where T : class
        {
            int SetValueAbsoluteExpiration = 0, SetValueSlidingExpiratio = 0;
            SetValueAbsoluteExpiration = SetAbsoluteExpiration;
            SetValueSlidingExpiratio = SetSlidingExpiration;

            var data = dbData;
            var jsonString = JsonConvert.SerializeObject(data);
            byte[] encodedCurrentTimeUTC = Encoding.UTF8.GetBytes(jsonString);
            // Setting up the cache options
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddSeconds(SetValueAbsoluteExpiration))
                .SetSlidingExpiration(TimeSpan.FromMinutes(SetValueSlidingExpiratio));
            _distributedCache.Set(query, encodedCurrentTimeUTC, options);
            return data;
        }
        public IEnumerable<T> AddDataToCache<T>(string query, IEnumerable<T> dbData) where T : class
        {
            var data = dbData;
            var jsonString = JsonConvert.SerializeObject(data);
            byte[] encodedCurrentTimeUTC = Encoding.UTF8.GetBytes(jsonString);
            // Setting up the cache options
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddMinutes(_cacheExpiration.SetAbsoluteExpiration))
                .SetSlidingExpiration(TimeSpan.FromMinutes(_cacheExpiration.SetSlidingExpiration));
            _distributedCache.Set(query, encodedCurrentTimeUTC, options);
            return data;
        }
        public IEnumerable<T> AddDataToCache<T>(string query, IEnumerable<T> dbData, int SetAbsoluteExpiration, int SetSlidingExpiration, string getCacheExpTimefor) where T : class
        {
            int SetValueAbsoluteExpiration = 0, SetValueSlidingExpiratio = 0;
            if (getCacheExpTimefor.ToLower() != "m")
            {
                SetValueAbsoluteExpiration = int.Parse(_configuration.GetSection(getCacheExpTimefor)["SetAbsoluteExpiration"]);
                SetValueSlidingExpiratio = int.Parse(_configuration.GetSection(getCacheExpTimefor)["SetSlidingExpiration"]);
            }
            else
            {
                SetValueAbsoluteExpiration = SetAbsoluteExpiration;
                SetValueSlidingExpiratio = SetSlidingExpiration;
            }
            var data = dbData;
            var jsonString = JsonConvert.SerializeObject(data);
            byte[] encodedCurrentTimeUTC = Encoding.UTF8.GetBytes(jsonString);
            // Setting up the cache options
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddMinutes(SetValueAbsoluteExpiration))
                .SetSlidingExpiration(TimeSpan.FromMinutes(SetValueSlidingExpiratio));
            _distributedCache.Set(query, encodedCurrentTimeUTC, options);
            return data;
        }
        public IEnumerable<T> DataFromCache<T>(string query) where T : class
        {
            var CacheData = _distributedCache.Get(query);
            var serializedList = Encoding.UTF8.GetString(CacheData);
            var data = JsonConvert.DeserializeObject<IEnumerable<T>>(serializedList);
            return data;
        }

        public bool RemoveCache(string query)
        {
            if (hasCache(query))
            {
                _distributedCache.Remove(query);
            }
            return true;
        }

        public bool AddActiveCacheKey(string CacheKeybase, string key, string UserId, bool IsRemoveReq)
        {
            List<ActiveCacheInfo> _ActiveCacheInfo = new List<ActiveCacheInfo>();
            if (hasCache("ActiveCacheKey"))
            {
                _ActiveCacheInfo = DataFromCache<ActiveCacheInfo>("ActiveCacheKey").ToList();
            }
            _ActiveCacheInfo.Add(new ActiveCacheInfo { CacheKeybase = CacheKeybase, key = key, userId = UserId, IsRemoveReq = IsRemoveReq });
            var data = _ActiveCacheInfo;
            var jsonString = JsonConvert.SerializeObject(data);
            byte[] encodedCurrentTimeUTC = Encoding.UTF8.GetBytes(jsonString);
            // Setting up the cache options
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddMinutes(1440))
                .SetSlidingExpiration(TimeSpan.FromMinutes(1440));
            _distributedCache.Set("ActiveCacheKey", encodedCurrentTimeUTC, options);
            return true;
        }

    }
}
