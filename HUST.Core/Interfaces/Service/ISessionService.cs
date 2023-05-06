using HUST.Core.Models.ServerObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HUST.Core.Interfaces.Service
{
    /// <summary>
    /// Interface BL xử lý session
    /// </summary>
    public interface ISessionService
    {
        /// <summary>
        /// Set cache
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="token"></param>
        void SetToken(string sessionId, string token);

        /// <summary>
        /// Get cache
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        string GetToken(string sessionId);

        /// <summary>
        /// Xóa cache
        /// </summary>
        void RemoveToken(string sessionId);
    }
}
