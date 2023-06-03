using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    /// <summary>
    /// Bảng concept: Bảng chứa thông tin concept
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.Table("concept")]
    public class concept : BaseEntity
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [System.ComponentModel.DataAnnotations.Key, ExplicitKey]
        public Guid concept_id { get; set; }

        /// <summary>
        /// Id dictionary
        /// </summary>
        public Guid? dictionary_id { get; set; }

        /// <summary>
        /// Tên
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// Mô tả/định nghĩa
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Title được chuẩn hóa
        /// </summary>
        public string normalized_title { get; set; }

        /// <summary>
        /// Giá trị soundex của title
        /// </summary>
        public string soundex_title { get; set; }
    }
}
