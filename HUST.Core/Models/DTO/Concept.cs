using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    /// <summary>
    /// Bảng concept: Bảng chứa thông tin concept
    /// </summary>
    public class Concept : BaseDTO
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [Key]
        public Guid ConceptId { get; set; }

        /// <summary>
        /// Id dictionary
        /// </summary>
        public Guid? DictionaryId { get; set; }

        /// <summary>
        /// Tên
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Mô tả/định nghĩa
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Title được chuẩn hóa
        /// </summary>
        public string NormalizedTitle { get; set; }

        /// <summary>
        /// Giá trị soundex của title
        /// </summary>
        public string SoundexTitle { get; set; }
    }
}
