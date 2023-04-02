using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    /// <summary>
    /// Bảng user: Bảng thông tin user
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.Table("user")]
    public class user : BaseEntity
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [System.ComponentModel.DataAnnotations.Key, ExplicitKey]
        public Guid user_id { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// URL ảnh đại diện
        /// </summary>
        public string avatar { get; set; }

        /// <summary>
        /// Tên người dùng
        /// </summary>
        public string full_name { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string email { get; set; }
    }
}
