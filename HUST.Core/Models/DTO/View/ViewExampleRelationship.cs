using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    public class ViewExampleRelationship
    {
        public Guid? DictionaryId { get; set; }
        public Guid? ConceptId { get; set; }
        public string Concept { get; set; }
        public Guid? ExampleId { get; set; }
        public string Example { get; set; }
        public string ExampleHtml { get; set; }
        public Guid? ExampleLinkId { get; set; }
        public string ExampleLinkName { get; set; }
        public DateTime? ConceptCreatedDate { get; set; }
        public DateTime? ExampleCreatedDate { get; set; }
        public DateTime? RelationCreatedDate { get; set; }
    }
}
