using HUST.Core.Constants;
using HUST.Core.Enums;
using HUST.Core.Interfaces.Service;
using HUST.Core.Interfaces.Util;
using HUST.Core.Models.DTO;
using HUST.Core.Models.Param;
using HUST.Core.Models.ServerObject;
using HUST.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HUST.Api.Controllers
{
    /// <summary>
    /// Lớp controller cung cấp api liên quan đến dashboard
    /// </summary>
    public class UserSettingController : BaseApiController
    {
        #region Fields
        private readonly IUserSettingService _service;
        #endregion

        #region Constructors

        public UserSettingController(IHustServiceCollection serviceCollection,
            IUserSettingService service) : base(serviceCollection)
        {
            _service = service;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Lấy setting theo key
        /// </summary>
        /// <param name="settingKey"></param>
        /// <returns></returns>
        [HttpGet("get_user_setting_by_key")]
        public async Task<IServiceResult> GetUserSettingByKey([FromQuery]string settingKey)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.GetUserSettingByKey(settingKey);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Thêm/Sửa setting theo key
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("save_user_setting_with_key")]
        public async Task<IServiceResult> SaveUserSettingWithKey([FromBody] UserSetting param)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.InsertUpdateUserSettingWithKey(param.SettingKey, param.SettingValue);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }
        #endregion
    }
}
