using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    /// <summary>
    /// Bảng audit_log: Bảng chứa thông tin lịch sử truy cập
    /// </summary>
    public class AuditLog : BaseDTO
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [Key]
        public int AuditLogId { get; set; }

        /// <summary>
        /// Id người dùng
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Thông tin màn hình/Tên màn hình
        /// </summary>
        public string ScreenInfo { get; set; }

        /// <summary>
        /// Loại hành động
        /// </summary>
        public int? ActionType { get; set; }

        /// <summary>
        /// Thông tin tham chiếu, vd: id dictionary đang thao tác
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Thông tin user agent
        /// </summary>
        public string UserAgent { get; set; }

    }
}
