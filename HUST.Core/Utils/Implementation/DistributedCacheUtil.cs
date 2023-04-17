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
    /// Lớp hỗ trợ DistributedCache
    /// </summary>
    public class DistributedCacheUtil : IDistributedCacheUtil
    {
        //private readonly IHustDistributedCache _cache;
        private readonly IDistributedCache _cache;
        private readonly IWebHostEnvironment _environment;

        public DistributedCacheUtil(IDistributedCache cache, IWebHostEnvironment environment)
        {
            _cache = cache;
            _environment = environment;
        }

        /// <summary>
        /// Thêm string vào cache
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="value">Giá trị cần cache</param>
        /// <param name="timeout">Thời gian hết hạn</param>
        /// <param name="isAbsoluteExpiraton"></param>
        /// <returns></returns>
        public async Task SetStringAsync(string key, string value, TimeSpan timeout, bool isAbsoluteExpiraton = false)
        {
            key = $"{_environment.EnvironmentName}_{key}";
            var options = new DistributedCacheEntryOptions();
            if(isAbsoluteExpiraton)
            {
                options.SetAbsoluteExpiration(timeout);
            } else
            {
                options.SetSlidingExpiration(timeout);
            }
            await _cache.SetStringAsync(key, value, options);
        }

        /// <summary>
        /// Thêm string vào cache, với thời gian mặc định = 20 phút
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="isAbsoluteExpiraton"></param>
        /// <returns></returns>
        public async Task SetStringAsync(string key, string value, bool isAbsoluteExpiraton = false)
        {
            await SetStringAsync(key, value, TimeSpan.FromMinutes(20), isAbsoluteExpiraton);
        }

        /// <summary>
        /// Thêm object vào cache
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="value">Giá trị cần cache</param>
        /// <param name="timeout">Thời gian hết hạn</param>
        /// <param name="isAbsoluteExpiraton"></param>
        /// <returns></returns>
        public async Task SetAsync(string key, object value, TimeSpan timeout, bool isAbsoluteExpiraton = false)
        {
            key = $"{_environment.EnvironmentName}_{key}";
            var options = new DistributedCacheEntryOptions();
            if (isAbsoluteExpiraton)
            {
                options.SetAbsoluteExpiration(timeout);
            }
            else
            {
                options.SetSlidingExpiration(timeout);
            }
            await _cache.SetAsync(key, value.ToBytes(), options);
        }

        /// <summary>
        /// Thêm object vào cache, với thời gian mặc định = 20 phút
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="value">Giá trị cần cache</param>
        /// <param name="timeout">Thời gian hết hạn</param>
        /// <param name="isAbsoluteExpiraton"></param>
        /// <returns></returns>
        public async Task SetAsync(string key, object value, bool isAbsoluteExpiraton = false)
        {
            await SetAsync(key, value, TimeSpan.FromMinutes(20), isAbsoluteExpiraton);
        }

        /// <summary>
        /// Xóa giá trị trong cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task DeleteAsync(string key)
        {
            key = $"{_environment.EnvironmentName}_{key}";
            await _cache.RemoveAsync(key);
        }

        /// <summary>
        /// Lấy giá trị string trong cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> GetStringAsync(string key)
        {
            key = $"{_environment.EnvironmentName}_{key}";
            return await _cache.GetStringAsync(key);
        }

        /// <summary>
        /// Lấy object trong cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key)
        {
            key = $"{_environment.EnvironmentName}_{key}";
            var bytes = await _cache.GetAsync(key);
            return bytes.ToObject<T>();
        }


        /// <summary>
        /// Thêm object vào cache
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="value">Giá trị cần cache</param>
        /// <param name="timeout">Thời gian hết hạn</param>
        /// <param name="isAbsoluteExpiraton"></param>
        /// <returns></returns>
        public void Set(string key, object value, TimeSpan timeout, bool isAbsoluteExpiraton = false)
        {
            key = $"{_environment.EnvironmentName}_{key}";
            var options = new DistributedCacheEntryOptions();
            if (isAbsoluteExpiraton)
            {
                options.SetAbsoluteExpiration(timeout);
            }
            else
            {
                options.SetSlidingExpiration(timeout);
            }
            _cache.Set(key, value.ToBytes(), options);
        }

        /// <summary>
        /// Lấy object trong cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            key = $"{_environment.EnvironmentName}_{key}";
            var bytes = _cache.Get(key);
            return bytes.ToObject<T>();
        }

        /// <summary>
        /// Xóa giá trị trong cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void Delete(string key)
        {
            key = $"{_environment.EnvironmentName}_{key}";
            _cache.Remove(key);
        }
    }
}
