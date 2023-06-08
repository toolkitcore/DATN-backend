using HUST.Core.Models.DTO;
using HUST.Core.Models.ServerObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HUST.Core.Interfaces.Service
{
    /// <summary>
    /// Service xử lý config (concept link, example link, tone, mode...)
    /// </summary>
    public interface IUserConfigService
    {
        /// <summary>
        /// Lấy danh sách tất cả config: 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<UserConfigData> GetAllConfigData(string userId = null);

        /// <summary>
        /// Lấy danh sách concept link
        /// </summary>
        /// <returns></returns>
        Task<IServiceResult> GetListConceptLink();

    }
}
