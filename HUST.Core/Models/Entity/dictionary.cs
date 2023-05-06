using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    /// <summary>
    /// Bảng dictionary: Bảng chứa thông tin dictionary
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.Table("dictionary")]
    public class dictionary : BaseEntity
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [System.ComponentModel.DataAnnotations.Key, ExplicitKey]
        public Guid dictionary_id { get; set; }

        /// <summary>
        /// Id người dùng
        /// </summary>
        public Guid? user_id { get; set; }

        /// <summary>
        /// Tên
        /// </summary>
        public string dictionary_name { get; set; }

        /// <summary>
        /// Lần cuối truy cập
        /// </summary>
        public DateTime? last_view_at { get; set; }
    }
}
