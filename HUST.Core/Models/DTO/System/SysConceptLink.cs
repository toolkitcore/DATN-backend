using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    /// <summary>
    /// Bảng sys_ConceptLink: Bảng thông tin liên kết concept-concept mặc định của hệ thống
    /// </summary>
    public class SysConceptLink : BaseDTO
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [Key]
        public Guid SysConceptLinkId { get; set; }

        /// <summary>
        /// Tên
        /// </summary>
        public string ConceptLinkName { get; set; }

        /// <summary>
        /// Loại
        /// </summary>
        public int? ConceptLinkType { get; set; }

        /// <summary>
        /// Thứ tự sx
        /// </summary>
        public int? SortOrder { get; set; }
    }
}
