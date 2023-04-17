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
        Task<IServiceResult> Login(string userName, string password);
        Task<IServiceResult> Logout();
    }
}
