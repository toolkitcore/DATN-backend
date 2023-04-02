using Dapper.Contrib.Extensions;
using HUST.Core.Enums;
using System;

namespace HUST.Core.Models.DTO
{
    /// <summary>
    /// Bảng user: Bảng thông tin user
    /// </summary>
    public class User : BaseDTO
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [Key]
        public Guid UserId { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Tên người dùng
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// URL ảnh đại diện
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Trạng thái tài khoản
        /// </summary>
        public UserStatus Status { get; set; }
    }
}
