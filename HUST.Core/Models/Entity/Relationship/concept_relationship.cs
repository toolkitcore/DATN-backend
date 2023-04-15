using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    /// <summary>
    /// Bảng concept_relationship: Bảng chứa liên kết concept-concept
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.Table("concept_relationship")]
    public class concept_relationship : BaseEntity
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [System.ComponentModel.DataAnnotations.Key, ExplicitKey]
        public int concept_relationship_id { get; set; }

        /// <summary>
        /// Id dictionary
        /// </summary>
        public Guid? dictionary_id { get; set; }

        /// <summary>
        /// Id concept con
        /// </summary>
        public Guid? concept_id { get; set; }

        /// <summary>
        /// Id concept cha
        /// </summary>
        public Guid? parent_id { get; set; }

        /// <summary>
        /// Id loại liên kết concept-concept
        /// </summary>
        public Guid? concept_link_id { get; set; }
    }
}
