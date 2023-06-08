using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Utils
{
    public interface ICacheSqlUtil
    {
        /// <summary>
        /// Thêm dữ liệu vào cahce cache
        /// </summary>
        /// <param name="cacheKey">Cache key</param>
        /// <param name="cacheValue">Giá trị cần cache</param>
        /// <param name="cacheType">Loại cache</param>
        /// <param name="timeout">Thời gian hết hạn</param>
        /// <returns></returns>
        Task<bool> SetCache(string cacheKey, string cacheValue, int? cacheType, TimeSpan? timeout = null);

        /// <summary>
        /// Xóa giá trị trong cache
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        Task<bool> DeleteCache(string cacheKey);

        /// <summary>
        /// Xóa giá trị trong cache với tham số truyền vào
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<bool> DeleteCache(object param);

        /// <summary>
        /// Lấy dữ liệu trong cache
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        Task<string> GetCache(string cacheKey);
    }
}
