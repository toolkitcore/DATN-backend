using HUST.Core.Constants;

namespace HUST.Core.Models.ServerObject
{
    /// <summary>
    /// Đối tượng xuất/nhập khẩu: map với view_concept_relationship
    /// </summary>
    public class ConceptRelationshipImport : BaseImport
    {
        [ImportColumn(TemplateConfig.ConceptRelationshipSheet.ChildConcept)]
        public string ChildName { get; set; }

        [ImportColumn(TemplateConfig.ConceptRelationshipSheet.ParentConcept)]
        public string ParentName { get; set; }

        [ImportColumn(TemplateConfig.ConceptRelationshipSheet.Relation)]
        public string ConceptLinkName { get; set; }
    }
}
