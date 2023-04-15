using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    /// <summary>
    /// Bảng register: Bảng thông tin register
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.Table("register")]
    public class register : BaseEntity
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [System.ComponentModel.DataAnnotations.Key, ExplicitKey]
        public Guid register_id { get; set; }

        /// <summary>
        /// Id bảng sys
        /// </summary>
        public Guid? sys_register_id { get; set; }

        /// <summary>
        /// Id user
        /// </summary>
        public Guid? user_id { get; set; }

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
