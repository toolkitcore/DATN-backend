using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    /// <summary>
    /// Bảng tone: Bảng thông tin tone
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.Table("tone")]
    public class tone : BaseEntity
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [System.ComponentModel.DataAnnotations.Key, ExplicitKey]
        public Guid tone_id { get; set; }

        /// <summary>
        /// Id bảng sys
        /// </summary>
        public Guid? sys_tone_id { get; set; }

        /// <summary>
        /// Id user
        /// </summary>
        public Guid? user_id { get; set; }

        /// <summary>
        /// Tên
        /// </summary>
        public string tone_name { get; set; }

        /// <summary>
        /// Loại
        /// </summary>
        public int? tone_type { get; set; }

        /// <summary>
        /// Thứ tự sx
        /// </summary>
        public int? sort_order { get; set; }
    }
}
