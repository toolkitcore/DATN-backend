using HUST.Core.Models.DTO;
using HUST.Core.Models.Entity;
using System;
using System.Threading.Tasks;

namespace HUST.Core.Interfaces.Repository
{

    public interface IUserRepository: IBaseRepository<user>
    {
        /// <summary>
        /// Khởi tạo dữ liệu
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> CreateActivatedAccountData(string userId);
    }
}
