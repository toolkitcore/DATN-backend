using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    /// <summary>
    /// Bảng user_setting: Bảng thông tin cấu hình của người dùng
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.Table("user_setting")]
    public class user_setting : BaseEntity
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [System.ComponentModel.DataAnnotations.Key, ExplicitKey]
        public Guid user_setting_id { get; set; }

        /// <summary>
        /// Id người dùng
        /// </summary>
        public Guid? user_id { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        public string setting_key { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public string setting_value { get; set; }
    }
}
