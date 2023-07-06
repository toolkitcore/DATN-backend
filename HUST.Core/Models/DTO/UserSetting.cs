using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    /// <summary>
    /// Bảng user_setting: Bảng thông tin cấu hình của người dùng
    /// </summary>
    public class UserSetting : BaseDTO
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [Key]
        public int UserSettingId { get; set; }

        /// <summary>
        /// Id người dùng
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        public string SettingKey { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public string SettingValue { get; set; }

        /// <summary>
        /// Là thiết lập của hệ thống
        /// </summary>
        public bool? IsSystem { get; set; }
    }
}
