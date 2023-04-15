using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    /// <summary>
    /// Bảng sys_nuance: Bảng thông tin nuance mặc định của hệ thống
    /// </summary>
    public class SysNuance : BaseDTO
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [Key]
        public Guid SysNuanceId { get; set; }

        /// <summary>
        /// Tên
        /// </summary>
        public string NuanceName { get; set; }

        /// <summary>
        /// Loại
        /// </summary>
        public int? NuanceType { get; set; }

        /// <summary>
        /// Thứ tự sx
        /// </summary>
        public int? SortOrder { get; set; }
    }
}
