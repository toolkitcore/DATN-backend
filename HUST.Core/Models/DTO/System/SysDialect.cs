using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    /// <summary>
    /// Bảng sys_dialect: Bảng thông tin dialect mặc định của hệ thống
    /// </summary>
    public class SysDialect : BaseDTO
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [Key]
        public Guid SysDialectId { get; set; }

        /// <summary>
        /// Tên
        /// </summary>
        public string DialectName { get; set; }

        /// <summary>
        /// Loại
        /// </summary>
        public int? DialectType { get; set; }

        /// <summary>
        /// Thứ tự sx
        /// </summary>
        public int? SortOrder { get; set; }
    }
}
