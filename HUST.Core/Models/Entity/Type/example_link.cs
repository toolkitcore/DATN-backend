using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    /// <summary>
    /// Bảng example_link: Bảng chứa thông tin loại liên kết example-content
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.Table("example_link")]
    public class example_link : BaseEntity
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [System.ComponentModel.DataAnnotations.Key, ExplicitKey]
        public Guid example_link_id { get; set; }

        /// <summary>
        /// Sys id
        /// </summary>
        public Guid? sys_example_link_id { get; set; }

        /// <summary>
        /// Id người dùng
        /// </summary>
        public Guid? user_id { get; set; }

        /// <summary>
        /// Tên
        /// </summary>
        public string example_link_name { get; set; }

        /// <summary>
        /// Loại
        /// </summary>
        public int? example_link_type { get; set; }

        /// <summary>
        /// Thứ tự sx
        /// </summary>
        public int? sort_order { get; set; }
    }
}
