using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    /// <summary>
    /// Bảng nuance: Bảng thông tin nuance
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.Table("nuance")]
    public class nuance : BaseEntity
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [System.ComponentModel.DataAnnotations.Key, ExplicitKey]
        public Guid nuance_id { get; set; }

        /// <summary>
        /// Id bảng sys
        /// </summary>
        public Guid? sys_nuance_id { get; set; }

        /// <summary>
        /// Id user
        /// </summary>
        public Guid? user_id { get; set; }

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
