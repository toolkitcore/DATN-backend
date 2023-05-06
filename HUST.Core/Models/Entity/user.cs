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
        /// Tên đăng nhập
        /// </summary>
        public string user_name { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// Tên đầy đủ
        /// </summary>
        public string full_name { get; set; }

        /// <summary>
        /// Tên hiển thị
        /// </summary>
        public string display_name { get; set; }

        /// <summary>
        /// Vị trí, công việc
        /// </summary>
        public string position { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public DateTime? birthday { get; set; }

        /// <summary>
        /// Link avatar
        /// </summary>
        public string avatar { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int? status { get; set; }
    }
}
