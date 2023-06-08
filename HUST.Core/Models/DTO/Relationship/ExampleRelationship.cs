using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    /// <summary>
    /// Bảng example_relationship: Bảng chứa liên kết example-content
    /// </summary>
    public class ExampleRelationship : BaseDTO
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [Key]
        public int ExampleRelationshipId { get; set; }

        /// <summary>
        /// Id dictionary
        /// </summary>
        public Guid? DictionaryId { get; set; }

        /// <summary>
        /// Id concept
        /// </summary>
        public Guid? ConceptId { get; set; }

        /// <summary>
        /// Id example
        /// </summary>
        public Guid? ExampleId { get; set; }

        /// <summary>
        /// Id loại liên kết example-content
        /// </summary>
        public Guid? ExampleLinkId { get; set; }
    }
}
