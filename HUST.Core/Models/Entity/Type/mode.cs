using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    /// <summary>
    /// Bảng mode: Bảng thông tin mode
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.Table("mode")]
    public class mode : BaseEntity
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [System.ComponentModel.DataAnnotations.Key, ExplicitKey]
        public Guid mode_id { get; set; }

        /// <summary>
        /// Id bảng sys
        /// </summary>
        public Guid? sys_mode_id { get; set; }

        /// <summary>
        /// Id user
        /// </summary>
        public Guid? user_id { get; set; }

        /// <summary>
        /// Tên
        /// </summary>
        public string mode_name { get; set; }

        /// <summary>
        /// Loại
        /// </summary>
        public int? mode_type { get; set; }

        /// <summary>
        /// Thứ tự sx
        /// </summary>
        public int? sort_order { get; set; }
    }
}
