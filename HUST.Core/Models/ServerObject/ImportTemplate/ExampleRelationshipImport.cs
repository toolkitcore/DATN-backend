using HUST.Core.Constants;

namespace HUST.Core.Models.ServerObject
{
    /// <summary>
    /// Đối tượng xuất/nhập khẩu: map với view_example_relationship
    /// </summary>
    public class ExampleRelationshipImport : BaseImport
    {
        [ImportColumn(TemplateConfig.ExampleRelationshipSheet.Example)]
        public string ExampleHtml { get; set; }

        [ImportColumn(TemplateConfig.ExampleRelationshipSheet.Concept)]
        public string Concept { get; set; }

        [ImportColumn(TemplateConfig.ExampleRelationshipSheet.Relation)]
        public string ExampleLinkName { get; set; }
    }
}
