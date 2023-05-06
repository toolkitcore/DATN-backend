using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    /// <summary>
    /// Bảng concept_link: Bảng chứa thông tin loại liên kết concept-concept
    /// </summary>
    public class ConceptLink : BaseDTO
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [Key]
        public Guid ConceptLinkId { get; set; }

        /// <summary>
        /// Id sys
        /// </summary>
        public Guid? SysConceptLinkId { get; set; }

        /// <summary>
        /// Id người dùng
        /// </summary>
        public Guid? UserId { get; set; }

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
