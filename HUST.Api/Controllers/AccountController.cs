using HUST.Core.Interfaces.Service;
using HUST.Core.Models.Param;
using HUST.Core.Models.ServerObject;
using HUST.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HUST.Api.Controllers
{
    /// <summary>
    /// Lớp controller cung cấp api liên quan đến tài khoản
    /// </summary>
    public class AccountController : BaseApiController
    {
        #region Fields
        private readonly IAccountService _service;
        #endregion

        #region Constructors

        public AccountController(IHustServiceCollection serviceCollection, IAccountService service) : base(serviceCollection)
        {
            _service = service;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Trả về thông tin kiểm tra đã đăng nhập hay chưa
        /// </summary>
        /// <returns></returns>
        [HttpGet("check_authenticate"), AllowAnonymous]
        public Task<IServiceResult> CheckAuthen()
        {
            IServiceResult res = new ServiceResult();
            try
            {
                var userId = this.ServiceCollection.AuthUtil.GetCurrentUserId();
                if (userId == null || userId == Guid.Empty)
                {
                    res.Data = false;
                } else
                {
                    res.Data = true;
                }
                return Task.FromResult(res);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return Task.FromResult(res); ;
        }

        /// <summary>
        /// Cho phép đăng ký một tài khoản mới
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("register"), AllowAnonymous]
        public async Task<IServiceResult> Register([FromBody] AccountParam param)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.Register(param.UserName, param.Password);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Thực hiện gửi 1 email hệ thống, chứa link kích hoạt tài khoản tới địa chỉ email mà người dùng cung cấp
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("send_activate_email"), AllowAnonymous]
        public async Task<IServiceResult> SendActivateEmail([FromBody] AccountParam param)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.SendActivateEmail(param.UserName, param.Password);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Thực hiện kích hoạt tài khoản người dùng
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet("activate_account"), AllowAnonymous]
        public async Task<IServiceResult> ActivateAccount(string token)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.ActivateAccount(token);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("login"), AllowAnonymous]
        public async Task<IServiceResult> Login([FromBody] AccountParam param)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.Login(param.UserName, param.Password);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        /// <returns></returns>
        [HttpGet("logout")]
        public async Task<IServiceResult> Logout()
        {
            var res = new ServiceResult();
            try
            {
                return await _service.Logout();
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Thực hiện gửi email hệ thống chứa link reset mật khẩu tới email mà người dung cung cấp
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet("forgot_password"), AllowAnonymous]
        public async Task<IServiceResult> ForgotPassword(string email)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.ForgotPassword(email);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Kiểm tra quyền truy cập trang reset mật khẩu
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet("check_access_reset_password"), AllowAnonymous]
        public async Task<IServiceResult> CheckAccessResetPassword(string token)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.CheckAccessResetPassword(token);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Reset mật khẩu cho người dùng quên mật khẩu
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPut("reset_password"), AllowAnonymous]
        public async Task<IServiceResult> ResetPassword([FromBody]PasswordParam param)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.ResetPassword(param.Token, param.NewPassword);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Lấy thông tin tài khoản: user + dictionary...
        /// </summary>
        /// <returns></returns>
        [HttpGet("get_account_info")]
        public async Task<IServiceResult> GetAccountInfo()
        {
            var res = new ServiceResult();
            try
            {
                return await _service.GetAccountInfo();
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
