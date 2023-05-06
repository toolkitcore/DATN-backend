using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    /// <summary>
    /// Bảng sys_mode: Bảng thông tin mode mặc định của hệ thống
    /// </summary>
    public class SysMode : BaseDTO
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [Key]
        public Guid SysModeId { get; set; }

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
