using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    /// <summary>
    /// Bảng tone: Bảng thông tin tone
    /// </summary>
    public class Tone : BaseDTO
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [Key]
        public Guid ToneId { get; set; }

        /// <summary>
        /// Id bảng sys
        /// </summary>
        public Guid? SysToneId { get; set; }

        /// <summary>
        /// Id user
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Tên
        /// </summary>
        public string ToneName { get; set; }

        /// <summary>
        /// Loại
        /// </summary>
        public int? ToneType { get; set; }

        /// <summary>
        /// Thứ tự sx
        /// </summary>
        public int? SortOrder { get; set; }
    }
}
