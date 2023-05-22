using HUST.Core.Models.ServerObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HUST.Core.Interfaces.Service
{
    /// <summary>
    /// Interface BL xử lý account
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Xử lý đăng ký tài khoản
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<IServiceResult> Register(string userName, string password);

        /// <summary>
        /// Xử lý yêu cầu gửi email xác minh tài khoản
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<IServiceResult> SendActivateEmail(string userName, string password);

        /// <summary>
        /// Thực hiện kích hoạt tài khoản người dùng
        /// </summary>
        /// <param name="token">Token kích hoạt</param>
        /// <returns></returns>
        Task<IServiceResult> ActivateAccount(string token);

        /// <summary>
        /// Xử lý login
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<IServiceResult> Login(string userName, string password);

        /// <summary>
        /// Xử lý logout
        /// </summary>
        /// <returns></returns>
        Task<IServiceResult> Logout();


        /// <summary>
        /// Xử lý gửi email hệ thống chứa link reset mật khẩu tới email mà người dung cung cấp
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<IServiceResult> ForgotPassword(string email);

        /// <summary>
        /// Kiểm tra quyền truy cập trang reset mật khẩu
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IServiceResult> CheckAccessResetPassword(string token);

        /// <summary>
        /// Xử lý Reset mật khẩu cho người dùng quên mật khẩu.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        Task<IServiceResult> ResetPassword(string token, string newPassword);
    }
}
