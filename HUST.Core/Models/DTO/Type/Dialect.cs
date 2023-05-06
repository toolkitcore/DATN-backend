using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    /// <summary>
    /// Bảng dialect: Bảng thông tin dialect
    /// </summary>
    public class Dialect : BaseDTO
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [Key]
        public Guid DialectId { get; set; }

        /// <summary>
        /// Id bảng sys
        /// </summary>
        public Guid? SysDialectId { get; set; }

        /// <summary>
        /// Id user
        /// </summary>
        public Guid? UserId { get; set; }

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
