using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    /// <summary>
    /// Bảng nuance: Bảng thông tin nuance
    /// </summary>
    public class Nuance : BaseDTO
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [Key]
        public Guid NuanceId { get; set; }

        /// <summary>
        /// Id bảng sys
        /// </summary>
        public Guid? SysNuanceId { get; set; }

        /// <summary>
        /// Id user
        /// </summary>
        public Guid? UserId { get; set; }

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
