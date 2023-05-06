using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    /// <summary>
    /// Bảng sys_tone: Bảng thông tin tone mặc định của hệ thống
    /// </summary>
    public class SysTone : BaseDTO
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [Key]
        public Guid SysToneId { get; set; }

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
