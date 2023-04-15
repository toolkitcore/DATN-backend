using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    /// <summary>
    /// Bảng sys_register: Bảng thông tin register mặc định của hệ thống
    /// </summary>
    public class SysRegister : BaseDTO
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [Key]
        public Guid SysRegisterId { get; set; }

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
