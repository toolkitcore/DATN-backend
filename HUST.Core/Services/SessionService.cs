using HUST.Core.Constants;
using HUST.Core.Interfaces.Service;
using HUST.Core.Utils;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Services
{
    /// <summary>
    /// Serivce xử lý session
    /// </summary>
    public class SessionService : ISessionService
    {
        private readonly IDistributedCacheUtil _cache;
        private readonly IConfiguration _configuration;

        public SessionService(IDistributedCacheUtil cache, IConfiguration configuration)
        {
            _cache = cache;
            _configuration = configuration;
        }

        /// <summary>
        /// Lấy token từ key session_id
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public string GetToken(string sessionId)
        {
            var key = string.Format(CacheKey.SessionCacheKey, sessionId);
            return _cache.Get<string>(key);
        }

        /// <summary>
        /// Xóa token trong cache
        /// </summary>
        /// <param name="sessionId"></param>
        public void RemoveToken(string sessionId)
        {
            var key = string.Format(CacheKey.SessionCacheKey, sessionId);
            _cache.Delete(key);
        }

        /// <summary>
        /// Cache token bằng key session_id
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="token"></param>
        public void SetToken(string sessionId, string token)
        {
            var key = string.Format(CacheKey.SessionCacheKey, sessionId);
            var timeout = SecurityUtil.GetAuthTokenLifeTime(_configuration);
            _cache.Set(key, token, TimeSpan.FromMinutes(timeout));
        }

    }


}
