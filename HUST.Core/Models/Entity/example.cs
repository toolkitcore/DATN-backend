using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    /// <summary>
    /// Bảng example: Bảng chứa thông tin example
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.Table("example")]
    public class example : BaseEntity
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [System.ComponentModel.DataAnnotations.Key, ExplicitKey]
        public Guid example_id { get; set; }

        /// <summary>
        /// Id dictionary
        /// </summary>
        public Guid? dictionary_id { get; set; }

        /// <summary>
        /// Id tone
        /// </summary>
        public Guid? tone_id { get; set; }

        /// <summary>
        /// Id register
        /// </summary>
        public Guid? register_id { get; set; }

        /// <summary>
        /// Id dialect
        /// </summary>
        public Guid? dialect_id { get; set; }

        /// <summary>
        /// Id mode
        /// </summary>
        public Guid? mode_id { get; set; }

        /// <summary>
        /// Id nuance
        /// </summary>
        public Guid? nuance_id { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string note { get; set; }

        /// <summary>
        /// Nội dung ví dụ
        /// </summary>
        public string detail { get; set; }

        /// <summary>
        /// Nội dung ví dụ dạng html
        /// </summary>
        public string detail_html { get; set; }
    }
}
