using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    /// <summary>
    /// Bảng example_relationship: Bảng chứa liên kết example-content
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.Table("example_relationship")]
    public class example_relationship : BaseEntity
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [System.ComponentModel.DataAnnotations.Key, ExplicitKey]
        public int example_relationship_id { get; set; }

        /// <summary>
        /// Id dictionary
        /// </summary>
        public Guid? dictionary_id { get; set; }

        /// <summary>
        /// Id concept
        /// </summary>
        public Guid? concept_id { get; set; }

        /// <summary>
        /// Id example
        /// </summary>
        public Guid? example_id { get; set; }

        /// <summary>
        /// Id loại liên kết example-content
        /// </summary>
        public Guid? example_link_id { get; set; }
    }
}
