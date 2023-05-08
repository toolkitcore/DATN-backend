using HUST.Core.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Utils
{
    /// <summary>
    /// Các hàm util liên quan đến authen
    /// </summary>
    public interface IAuthUtil
    {
        /// <summary>
        /// Lấy ra đối tượng User đang đăng nhập
        /// </summary>
        /// <returns></returns>
        User GetCurrentUser();

        /// <summary>
        /// Lấy id user đang đăng nhập
        /// </summary>
        /// <returns></returns>
        Guid? GetCurrentUserId();

        /// <summary>
        /// Lấy id dictionary đang sử dụng
        /// </summary>
        /// <returns></returns>
        Guid? GetCurrentDictionaryId();

    }
}
