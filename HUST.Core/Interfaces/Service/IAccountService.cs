using HUST.Core.Models.DTO;
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

        /// <summary>
        /// Lấy thông tin tài khoản
        /// </summary>
        /// <returns></returns>
        Task<IServiceResult> GetAccountInfo();


        // TODO: 3 method bên dưới cần xem xét lại sự phù hợp với interface này

        /// <summary>
        /// Sinh session mới
        /// </summary>
        /// <param name="user"></param>
        string GenerateSession(User user);

        /// <summary>
        /// Xóa session cũ
        /// </summary>
        void RemoveCurrentSession();

        /// <summary>
        /// Set session cho request response hiện tại
        /// </summary>
        /// <param name="sessionId"></param>
        void SetResponseSessionCookie(string sessionId);

        #region Helper
        /// <summary>
        /// Lấy key cache throttle hạn chế thời gian call api
        /// </summary>
        /// <returns></returns>
        string GetThrottleCacheKey(string name);

        /// <summary>
        /// Kiểm tra thời gian cần chờ trước khi call api liên tục
        /// </summary>
        /// <param name="name"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        double GetThrottleTime(string name);

        /// <summary>
        /// Set thời gian chặn call api liên tục
        /// </summary>
        /// <param name="name"></param>
        /// <param name="seconds"></param>
        void SetThrottleTime(string name, int seconds = 120);
        #endregion
    }
}
