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
        /// Xử lý đăng ký tài khoản
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<IServiceResult> Register(string userName, string password);

        /// <summary>
        /// Kích hoạt tài khoản
        /// </summary>
        /// <param name="token">Token kích hoạt</param>
        /// <returns></returns>
        Task<IServiceResult> ActivateAccount(string token);
    }
}
