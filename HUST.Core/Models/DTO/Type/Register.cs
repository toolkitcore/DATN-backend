using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    /// <summary>
    /// Bảng register: Bảng thông tin register
    /// </summary>
    public class Register : BaseDTO
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [Key]
        public Guid RegisterId { get; set; }

        /// <summary>
        /// Id bảng sys
        /// </summary>
        public Guid? SysRegisterId { get; set; }

        /// <summary>
        /// Id user
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Tên
        /// </summary>
        public string RegisterName { get; set; }

        /// <summary>
        /// Loại
        /// </summary>
        public int? RegisterType { get; set; }

        /// <summary>
        /// Thứ tự sx
        /// </summary>
        public int? SortOrder { get; set; }
    }
}
