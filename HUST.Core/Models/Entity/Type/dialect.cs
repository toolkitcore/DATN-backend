using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    /// <summary>
    /// Bảng dialect: Bảng thông tin dialect
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.Table("dialect")]
    public class dialect : BaseEntity
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [System.ComponentModel.DataAnnotations.Key, ExplicitKey]
        public Guid dialect_id { get; set; }

        /// <summary>
        /// Id bảng sys
        /// </summary>
        public Guid? sys_dialect_id { get; set; }

        /// <summary>
        /// Id user
        /// </summary>
        public Guid? user_id { get; set; }

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
