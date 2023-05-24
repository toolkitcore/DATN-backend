using HUST.Core.Interfaces.Service;
using HUST.Core.Models.DTO;
using HUST.Core.Models.Param;
using HUST.Core.Models.ServerObject;
using HUST.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HUST.Api.Controllers
{
    public class UserController : BaseApiController
    {
        #region Fields
        private readonly IUserService _service;
        #endregion

        #region Constructor
        public UserController(IHustServiceCollection serviceCollection, IUserService service) : base(serviceCollection)
        {
            _service = service;
        }
        #endregion


        /// <summary>
        /// Lấy thông tin đăng nhập của user hiện tại
        /// </summary>
        /// <returns></returns>
        [HttpGet("me")]
        public Task<IServiceResult> GetMe()
        {
            IServiceResult res = new ServiceResult();
            try
            {
                var user = this.ServiceCollection.AuthUtil.GetCurrentUser();
                res.Data = new
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    DictionaryId = user.DictionaryId
                };
                return Task.FromResult(res);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return Task.FromResult(res); ;
        }

        /// <summary>
        /// Thay đổi mật khẩu
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPut("update_password")]
        public async Task<IServiceResult> UpdatePassword([FromBody] PasswordParam param)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.UpdatePassword(param.OldPassword, param.NewPassword);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Lấy thông tin cá nhân của người dùng
        /// </summary>
        /// <returns></returns>
        [HttpGet("get_user_info")]
        public async Task<IServiceResult> GetUserInfo()
        {
            var res = new ServiceResult();
            try
            {
                return await _service.GetUserInfo();
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Cập nhật các thông tin cá nhân của người dùng
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPatch("update_user_info")]
        public async Task<IServiceResult> UpdateUserInfo([FromForm] UpdateUserInfoParam param)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.UpdateUserInfo(param);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }
    }
}
