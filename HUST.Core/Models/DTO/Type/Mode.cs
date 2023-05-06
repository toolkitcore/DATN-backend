using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    /// <summary>
    /// Bảng mode: Bảng thông tin mode
    /// </summary>
    public class Mode : BaseDTO
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [Key]
        public Guid ModeId { get; set; }

        /// <summary>
        /// Id bảng sys
        /// </summary>
        public Guid? SysModeId { get; set; }

        /// <summary>
        /// Id user
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Tên
        /// </summary>
        public string ModeName { get; set; }

        /// <summary>
        /// Loại
        /// </summary>
        public int? ModeType { get; set; }

        /// <summary>
        /// Thứ tự sx
        /// </summary>
        public int? SortOrder { get; set; }
    }
}
