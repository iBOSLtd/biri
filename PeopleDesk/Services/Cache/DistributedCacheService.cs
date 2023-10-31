using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using PeopleDesk.Data;
using PeopleDesk.Models;
using System.Text;

namespace PeopleDesk.Services.Cache
{
    public class DistributedCacheService : IDistributedCacheService
    {
        private readonly IDistributedCache _distributedCache;
        public DistributedCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<IEnumerable<T>> AddDataToCache<T>(string key, IEnumerable<T> data, int setAbsoluteExpiration, int setSlidingExpiration, string casheExperiationType) where T : class
        {
            var jsonString = JsonConvert.SerializeObject(data);
            byte[] jsonData = Encoding.UTF8.GetBytes(jsonString);

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()

                .SetAbsoluteExpiration(
                  CasheExperiationTypeEnum.second.ToString() == casheExperiationType ? DateTime.Now.AddSeconds(setAbsoluteExpiration)
                : CasheExperiationTypeEnum.minute.ToString() == casheExperiationType ? DateTime.Now.AddMinutes(setAbsoluteExpiration)
                : CasheExperiationTypeEnum.hour.ToString() == casheExperiationType ? DateTime.Now.AddHours(setAbsoluteExpiration)
                : CasheExperiationTypeEnum.day.ToString() == casheExperiationType ? DateTime.Now.AddDays(setAbsoluteExpiration)
                : CasheExperiationTypeEnum.month.ToString() == casheExperiationType ? DateTime.Now.AddMonths(setAbsoluteExpiration)
                : CasheExperiationTypeEnum.year.ToString() == casheExperiationType ? DateTime.Now.AddYears(setAbsoluteExpiration)
                : DateTime.Now.AddMilliseconds(setAbsoluteExpiration))

                .SetAbsoluteExpiration(
                  CasheExperiationTypeEnum.second.ToString() == casheExperiationType ? TimeSpan.FromSeconds(setAbsoluteExpiration)
                : CasheExperiationTypeEnum.minute.ToString() == casheExperiationType ? TimeSpan.FromMinutes(setAbsoluteExpiration)
                : CasheExperiationTypeEnum.hour.ToString() == casheExperiationType ? TimeSpan.FromHours(setAbsoluteExpiration)
                : CasheExperiationTypeEnum.day.ToString() == casheExperiationType ? TimeSpan.FromDays(setAbsoluteExpiration)
                : TimeSpan.FromMilliseconds(setAbsoluteExpiration));


            await _distributedCache.SetAsync(key, jsonData, options);

            return data;
        }

        public async Task<IEnumerable<T>> DataFromCache<T>(string key) where T : class
        {
            var CacheData = await _distributedCache.GetAsync(key);
            var serializedList = Encoding.UTF8.GetString(CacheData);
            var data = JsonConvert.DeserializeObject<IEnumerable<T>>(serializedList);

            return data;
        }
        public async Task<bool> HasCache(string key)
        {
            var CacheData = await _distributedCache.GetAsync(key);
            if (CacheData != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> RemoveCache(string key)
        {
            if (await HasCache(key))
            {
                await _distributedCache.RemoveAsync(key);
            }
            return true;
        }




    }
}
