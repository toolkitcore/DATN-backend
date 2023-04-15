using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    /// <summary>
    /// Bảng concept_relationship: Bảng chứa liên kết concept-concept
    /// </summary>
    public class ConceptRelationship : BaseDTO
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [Key]
        public int ConceptRelationshipId { get; set; }

        /// <summary>
        /// Id dictionary
        /// </summary>
        public Guid? DictionaryId { get; set; }

        /// <summary>
        /// Id concept con
        /// </summary>
        public Guid? ConceptId { get; set; }

        /// <summary>
        /// Id concept cha
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Id loại liên kết concept-concept
        /// </summary>
        public Guid? ConceptLinkId { get; set; }
    }
}
