using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    /// <summary>
    /// Bảng sys_concept_link: Bảng thông tin liên kết concept-concept mặc định của hệ thống
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.Table("sys_concept_link")]
    public class sys_concept_link : BaseEntity
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [System.ComponentModel.DataAnnotations.Key, ExplicitKey]
        public Guid sys_concept_link_id { get; set; }

        /// <summary>
        /// Tên
        /// </summary>
        public string concept_link_name { get; set; }

        /// <summary>
        /// Loại
        /// </summary>
        public int? concept_link_type { get; set; }

        /// <summary>
        /// Thứ tự sx
        /// </summary>
        public int? sort_order { get; set; }
    }
}
