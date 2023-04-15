using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    /// <summary>
    /// Bảng concept_link: Bảng chứa thông tin loại liên kết concept-concept
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.Table("concept_link")]
    public class concept_link : BaseEntity
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [System.ComponentModel.DataAnnotations.Key, ExplicitKey]
        public Guid concept_link_id { get; set; }

        /// <summary>
        /// Id sys
        /// </summary>
        public Guid? sys_concept_link_id { get; set; }

        /// <summary>
        /// Id người dùng
        /// </summary>
        public Guid? user_id { get; set; }

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
