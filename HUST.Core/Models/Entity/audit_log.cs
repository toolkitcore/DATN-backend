using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    /// <summary>
    /// Bảng audit_log: Bảng chứa thông tin lịch sử truy cập
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.Table("audit_log")]
    public class audit_log : BaseEntity
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [System.ComponentModel.DataAnnotations.Key, ExplicitKey]
        public int audit_log_id { get; set; }

        /// <summary>
        /// Id người dùng
        /// </summary>
        public Guid? user_id { get; set; }

        /// <summary>
        /// Thông tin màn hình/Tên màn hình
        /// </summary>
        public string screen_info { get; set; }

        /// <summary>
        /// Loại hành động
        /// </summary>
        public int? action_type { get; set; }

        /// <summary>
        /// Thông tin tham chiếu, vd: id dictionary đang thao tác
        /// </summary>
        public string reference { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Thông tin user agent
        /// </summary>
        public string user_agent { get; set; }

    }
}
