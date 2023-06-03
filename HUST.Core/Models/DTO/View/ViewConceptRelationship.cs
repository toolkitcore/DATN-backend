using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    public class ViewConceptRelationship
    {
        public Guid? DictionaryId { get; set; }
        public Guid? ChildId { get; set; }
        public string ChildName { get; set; }
        public Guid? ParentId { get; set; }
        public string ParentName { get; set; }
        public Guid? ConceptLinkId { get; set; }
        public string ConceptLinkName { get; set; }
    }
}
