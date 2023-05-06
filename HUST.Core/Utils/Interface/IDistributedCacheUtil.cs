using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Utils
{
    /// <summary>
    /// Interface định nghĩa lớp distributed cache
    /// </summary>
    public interface IDistributedCacheUtil
    {

        /// <summary>
        /// Thêm string vào cache
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="value">Giá trị cần cache</param>
        /// <param name="timeout">Thời gian hết hạn</param>
        /// <param name="isAbsoluteExpiraton"></param>
        /// <returns></returns>
        Task SetStringAsync(string key, string value, TimeSpan timeout, bool isAbsoluteExpiraton = false);

        /// <summary>
        /// Thêm string vào cache, với thời gian mặc định = 20 phút
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="isAbsoluteExpiraton"></param>
        /// <returns></returns>
        Task SetStringAsync(string key, string value, bool isAbsoluteExpiraton = false);

        /// <summary>
        /// Thêm object vào cache
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="value">Giá trị cần cache</param>
        /// <param name="timeout">Thời gian hết hạn</param>
        /// <param name="isAbsoluteExpiraton"></param>
        /// <returns></returns>
        Task SetAsync(string key, object value, TimeSpan timeout, bool isAbsoluteExpiraton = false);

        /// <summary>
        /// Thêm object vào cache, với thời gian mặc định = 20 phút
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="value">Giá trị cần cache</param>
        /// <param name="timeout">Thời gian hết hạn</param>
        /// <param name="isAbsoluteExpiraton"></param>
        /// <returns></returns>
        Task SetAsync(string key, object value, bool isAbsoluteExpiraton = false);

        /// <summary>
        /// Xóa giá trị trong cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task DeleteAsync(string key);

        /// <summary>
        /// Lấy giá trị string trong cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<string> GetStringAsync(string key);

        /// <summary>
        /// Lấy object trong cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// Thêm object vào cache
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="value">Giá trị cần cache</param>
        /// <param name="timeout">Thời gian hết hạn</param>
        /// <param name="isAbsoluteExpiraton"></param>
        /// <returns></returns>
        void Set(string key, object value, TimeSpan timeout, bool isAbsoluteExpiraton = false);

        /// <summary>
        /// Lấy object trong cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key);

        /// <summary>
        /// Xóa giá trị trong cache
        /// </summary>
        /// <param name="key"></param>
        void Delete(string key);
    }
}
