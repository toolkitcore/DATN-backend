using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    /// <summary>
    /// Bảng sys_dialect: Bảng thông tin dialect mặc định của hệ thống
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.Table("sys_dialect")]
    public class sys_dialect : BaseEntity
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [System.ComponentModel.DataAnnotations.Key, ExplicitKey]
        public Guid sys_dialect_id { get; set; }

        /// <summary>
        /// Tên
        /// </summary>
        public string dialect_name { get; set; }

        /// <summary>
        /// Loại
        /// </summary>
        public int? dialect_type { get; set; }

        /// <summary>
        /// Thứ tự sx
        /// </summary>
        public int? sort_order { get; set; }
    }
}
