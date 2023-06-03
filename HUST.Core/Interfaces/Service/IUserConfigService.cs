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
    public interface IUserConfig
    {
        /// <summary>
        /// Lấy danh sách concept link
        /// </summary>
        /// <returns></returns>
        Task<IServiceResult> GetListConceptLink();

    }
}
