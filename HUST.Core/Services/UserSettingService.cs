using HUST.Core.Constants;
using HUST.Core.Enums;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Interfaces.Service;
using HUST.Core.Models.DTO;
using HUST.Core.Models.Entity;
using HUST.Core.Models.ServerObject;
using HUST.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Services
{
    /// <summary>
    /// Serivce xử lý user setting
    /// </summary>
    public class UserSettingService : BaseService, IUserSettingService
    {
        #region Field

        private readonly IUserSettingRepository _repository;

        #endregion

        #region Constructor

        public UserSettingService(IUserSettingRepository repository,
            IHustServiceCollection serviceCollection) : base(serviceCollection)
        {
            _repository = repository;
        }
        #endregion

        #region Method
        /// <summary>
        /// Lấy setting theo key
        /// </summary>
        /// <param name="settingKey"></param>
        /// <returns></returns>
        public async Task<IServiceResult> GetUserSettingByKey(string settingKey)
        {

            var res = new ServiceResult();

            if (string.IsNullOrEmpty(settingKey))
            {
                return res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
            }

            var setting = await _repository.SelectObject<UserSetting>(new
            {
                setting_key = settingKey,
                user_id = this.ServiceCollection.AuthUtil.GetCurrentUserId()
            }) as UserSetting;

            if(setting == null)
            {
                setting = await _repository.SelectObject<UserSetting>(new
                {
                    setting_key = settingKey,
                    is_system = true
                }) as UserSetting;
            }

            return res.OnSuccess(setting);
        }

        /// <summary>
        /// Thêm/Sửa setting theo key
        /// </summary>
        /// <param name="settingKey"></param>
        /// <param name="settingValue"></param>
        /// <returns></returns>
        public async Task<IServiceResult> InsertUpdateUserSettingWithKey(string settingKey, string settingValue)
        {
            var res = new ServiceResult();
            var userId = this.ServiceCollection.AuthUtil.GetCurrentUserId();

            if (string.IsNullOrEmpty(settingKey))
            {
                return res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
            }

            var setting = await _repository.SelectObject<UserSetting>(new
            {
                setting_key = settingKey,
                user_id = userId
            }) as UserSetting;

            var result = false;
            if(setting == null)
            {
                result = await _repository.Insert(new user_setting
                {
                    user_id = userId,
                    setting_key = settingKey,
                    setting_value = settingValue,
                    created_date = DateTime.Now
                });
            } 
            else
            {
                result = await _repository.Update(new
                {
                    user_setting_id = setting.UserSettingId,
                    setting_value = settingValue,
                    modified_date = DateTime.Now
                });
            }

            if(!result)
            {
                return res.OnError(ErrorCode.Err9999);
            }

            return res.OnSuccess();
        }


        #endregion

    }
}
