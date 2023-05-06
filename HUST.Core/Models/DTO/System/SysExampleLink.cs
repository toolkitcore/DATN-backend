using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    /// <summary>
    /// Bảng sys_example_link: Bảng thông tin liên kết example-concept mặc định của hệ thống
    /// </summary>
    public class SysExampleLink : BaseDTO
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [Key]
        public Guid SysExampleLinkId { get; set; }

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
