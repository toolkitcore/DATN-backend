using HUST.Core.Interfaces.Repository;
using HUST.Core.Models.DTO;
using HUST.Core.Models.Entity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Utils
{
    /// <summary>
    /// Lớp hỗ trợ cache dữ liệu vào database
    /// </summary>
    public class CacheSqlUtil : ICacheSqlUtil
    {
        private ICacheSqlRepository _repository;
        private IHustServiceCollection _serviceCollection;
        public CacheSqlUtil(ICacheSqlRepository repository, IHustServiceCollection serviceCollection)
        {
            _repository = repository;
            _serviceCollection = serviceCollection;
        }

        /// <summary>
        /// Thêm dữ liệu vào cahce cache
        /// </summary>
        /// <param name="cacheKey">Cache key</param>
        /// <param name="cacheValue">Giá trị cần cache</param>
        /// <param name="cacheType">Loại cache</param>
        /// <param name="timeout">Thời gian hết hạn</param>
        /// <returns></returns>
        public async Task<bool> SetCache(string cacheKey, string cacheValue, int? cacheType, TimeSpan? timeout = null)
        {
            if(timeout == null)
            {
                timeout = TimeSpan.FromMinutes(60);
            }
            var startTime = DateTime.Now;
            var endTime = startTime + timeout;
            var cache = new cache_sql
            {
                cache_key = cacheKey,
                value = cacheValue,
                user_id = this._serviceCollection.AuthUtil.GetCurrentUserId(),
                cache_type = cacheType,
                start_time = startTime,
                end_time = endTime
            };
            await _repository.Insert(cache);
            return true;
        }

        /// <summary>
        /// Xóa giá trị trong cache
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public async Task<bool> DeleteCache(string cacheKey)
        {
            await _repository.Delete(new {
                cache_key = cacheKey
            });
            return true;
        }

        /// <summary>
        /// Xóa giá trị trong cache với tham số truyền vào
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<bool> DeleteCache(object param)
        {
            await _repository.Delete(param);
            return true;
        }

        /// <summary>
        /// Lấy dữ liệu trong cache
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public async Task<string> GetCache(string cacheKey)
        {
            var cache = await _repository.SelectObject<CacheSql>(new
            {
                cache_key = cacheKey
            }) as CacheSql;
            return cache?.Value;
        }
    }
}
