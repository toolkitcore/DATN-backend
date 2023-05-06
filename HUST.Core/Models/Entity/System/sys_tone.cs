using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    /// <summary>
    /// Bảng sys_tone: Bảng thông tin tone mặc định của hệ thống
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.Table("sys_tone")]
    public class sys_tone : BaseEntity
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [System.ComponentModel.DataAnnotations.Key, ExplicitKey]
        public Guid sys_tone_id { get; set; }

        /// <summary>
        /// Tên
        /// </summary>
        public string tone_name { get; set; }

        /// <summary>
        /// Loại
        /// </summary>
        public int? tone_type { get; set; }

        /// <summary>
        /// Thứ tự sx
        /// </summary>
        public int? sort_order { get; set; }
    }
}
