using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    /// <summary>
    /// Bảng sys_nuance: Bảng thông tin nuance mặc định của hệ thống
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.Table("sys_nuance")]
    public class sys_nuance : BaseEntity
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [System.ComponentModel.DataAnnotations.Key, ExplicitKey]
        public Guid sys_nuance_id { get; set; }

        /// <summary>
        /// Tên
        /// </summary>
        public string nuance_name { get; set; }

        /// <summary>
        /// Loại
        /// </summary>
        public int? nuance_type { get; set; }

        /// <summary>
        /// Thứ tự sx
        /// </summary>
        public int? sort_order { get; set; }
    }
}
