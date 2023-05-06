using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    /// <summary>
    /// Bảng sys_register: Bảng thông tin register mặc định của hệ thống
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.Table("sys_register")]
    public class sys_register : BaseEntity
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [System.ComponentModel.DataAnnotations.Key, ExplicitKey]
        public Guid sys_register_id { get; set; }

        /// <summary>
        /// Tên
        /// </summary>
        public string register_name { get; set; }

        /// <summary>
        /// Loại
        /// </summary>
        public int? register_type { get; set; }

        /// <summary>
        /// Thứ tự sx
        /// </summary>
        public int? sort_order { get; set; }
    }
}
