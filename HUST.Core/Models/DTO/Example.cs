using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    /// <summary>
    /// Bảng example: Bảng chứa thông tin example
    /// </summary>
    public class Example : BaseDTO
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [Key]
        public Guid ExampleId { get; set; }

        /// <summary>
        /// Id dictionary
        /// </summary>
        public Guid? DictionaryId { get; set; }

        /// <summary>
        /// Id tone
        /// </summary>
        public Guid? ToneId { get; set; }

        /// <summary>
        /// Id register
        /// </summary>
        public Guid? RegisterId { get; set; }

        /// <summary>
        /// Id dialect
        /// </summary>
        public Guid? DialectId { get; set; }

        /// <summary>
        /// Id mode
        /// </summary>
        public Guid? ModeId { get; set; }

        /// <summary>
        /// Id nuance
        /// </summary>
        public Guid? NuanceId { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Nội dung ví dụ
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// Nội dung ví dụ dạng html
        /// </summary>
        public string DetailHtml { get; set; }
    }
}
