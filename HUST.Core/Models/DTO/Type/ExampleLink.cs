using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    /// <summary>
    /// Bảng xample_link: Bảng chứa thông tin loại liên kết example-content
    /// </summary>
    public class ExampleLink : BaseDTO
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [Key]
        public Guid ExampleLinkId { get; set; }

        /// <summary>
        /// Sys id
        /// </summary>
        public Guid? SysExampleLinkId { get; set; }

        /// <summary>
        /// Id người dùng
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Tên
        /// </summary>
        public string ExampleLinkName { get; set; }

        /// <summary>
        /// Loại
        /// </summary>
        public int? ExampleLinkType { get; set; }

        /// <summary>
        /// Thứ tự sx
        /// </summary>
        public int? SortOrder { get; set; }
    }
}
