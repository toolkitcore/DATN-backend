using HUST.Core.Models.DTO;
using HUST.Core.Models.ServerObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HUST.Core.Interfaces.Service
{
    /// <summary>
    /// Service xử lý user setting
    /// </summary>
    public interface IUserSettingService
    {
        /// <summary>
        /// Lấy setting theo key
        /// </summary>
        /// <param name="settingKey"></param>
        /// <returns></returns>
        Task<IServiceResult> GetUserSettingByKey(string settingKey);

        /// <summary>
        /// Thêm/Sửa setting theo key
        /// </summary>
        /// <param name="settingKey"></param>
        /// <param name="settingValue"></param>
        /// <returns></returns>
        Task<IServiceResult> InsertUpdateUserSettingWithKey(string settingKey, string settingValue);
    }
}
